namespace PSI_RENDU1
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;




    internal class Program
    {
        static void Main()
{
    var graphe = new Graphe<int>();

    Console.WriteLine("Voulez-vous :\n1. Charger le fichier métro\n2. Générer un graphe aléatoire ?");
    Console.Write("Entrez 1 ou 2 : ");
    string choix = Console.ReadLine();

    if (choix == "1")
    {
        string cheminExcel = "MetroParis_A.xlsx";
        if (!File.Exists(cheminExcel))
        {
            Console.WriteLine("Fichier introuvable");
            return;
        }

        // Charger les nœuds depuis Excel
        var donneesNoeuds = LecteurExcel.LireNoeuds(cheminExcel);
        foreach (var (id, nom, longitude, latitude) in donneesNoeuds)
        {
            graphe.AjouterNoeud(id, nom, longitude, latitude);
        }

        // Relier automatiquement les stations avec le même nom (ex : République)
        var groupesDoublons = donneesNoeuds
            .GroupBy(n => n.nom)
            .Where(g => g.Count() > 1);

        foreach (var groupe in groupesDoublons)
        {
            var ids = groupe.Select(n => n.id).ToList();
            for (int i = 0; i < ids.Count; i++)
            {
                for (int j = i + 1; j < ids.Count; j++)
                {
                    graphe.AjouterLien(ids[i], ids[j], 0.5); // temps fictif pour changement de ligne
                    graphe.AjouterLien(ids[j], ids[i], 0.5);
                }
            }
        }

        // Charger les arcs depuis Excel
        var arcs = LecteurExcel.LireArcs(cheminExcel);
        foreach (var (source, destination, poids, sensUnique) in arcs)
        {
            graphe.AjouterLien(source, destination, poids);
            if (!sensUnique)
                graphe.AjouterLien(destination, source, poids);
        }

        Console.WriteLine("✅ Graphe métro chargé avec succès.");
    }
    else if (choix == "2")
    {
        Console.Write("Combien de sommets pour le graphe ? ");
        int nombreSommets = int.Parse(Console.ReadLine());

        Console.Write("Combien d'arêtes pour le graphe ? ");
        int nombreAretes = int.Parse(Console.ReadLine());

        graphe.GenererGrapheAleatoire(nombreSommets, nombreAretes);
    }
    else
    {
        Console.WriteLine("Choix invalide");
        return;
    }

    Console.WriteLine("\nVoulez-vous afficher un parcours ?");
    Console.WriteLine("1. Parcours en profondeur (DFS)");
    Console.WriteLine("2. Parcours en largeur (BFS)");
    Console.Write("Entrez 1 ou 2 : ");
    string choixParcours = Console.ReadLine();

    Console.Write("Entrez le sommet de départ : ");
    int sommetDepart = int.Parse(Console.ReadLine());

    if (choixParcours == "1")
        graphe.ParcoursProfondeur(sommetDepart);
    else if (choixParcours == "2")
        graphe.ParcoursLargeur(sommetDepart);
    else
        Console.WriteLine("Choix invalide");

    Console.WriteLine(graphe.EstConnexe()
        ? "Le graphe est connexe"
        : "Le graphe n'est pas connexe");

    Console.WriteLine(graphe.ContientUnCycle()
        ? "Le graphe contient un ou plusieurs cycles"
        : "Le graphe est acyclique");

    Console.WriteLine("\nAffichage du graphe :");
    string imagePath = "graphe.png";
    GrapheVisualisation.GenererImageGraphe(graphe, imagePath);

    Console.WriteLine("\nVoulez-vous exécuter un algorithme de plus court chemin ?");
    Console.WriteLine("1. Dijkstra");
    Console.WriteLine("2. Bellman-Ford");
    Console.Write("Entrez 1 ou 2 : ");
    string choixAlgo = Console.ReadLine();

    if (choixAlgo == "1")
        ExecuterDijkstra(graphe);
    else if (choixAlgo == "2")
        ExecuterBellmanFord(graphe);
    else
        Console.WriteLine("Choix invalide");

    Console.WriteLine("\nAffichage du graphe chargé :");
    graphe.AfficherGraphe();
}

        static void ExecuterDijkstra<T>(Graphe<T> graphe)
        {
            Console.Write("\nEntrez le sommet de départ pour Dijkstra : ");
            string input = Console.ReadLine();

            T sommetDepart = (T)Convert.ChangeType(input, typeof(T));

            (Dictionary<T, double> distances, Dictionary<T, T?> precedent) = graphe.Dijkstra(sommetDepart);

            Console.WriteLine($"\nDistances minimales depuis le sommet {sommetDepart} (Dijkstra) :");
            foreach (var kvp in distances)
            {
                Console.WriteLine($"Vers {kvp.Key} : {kvp.Value}");
            }
        }

        static void ExecuterBellmanFord<T>(Graphe<T> graphe)
        {
            Console.Write("\nEntrez le sommet de départ pour Bellman-Ford : ");
            string input = Console.ReadLine();

            T sommetDepart = (T)Convert.ChangeType(input, typeof(T));

            try
            {
                (Dictionary<T, double> distances, Dictionary<T, T?> precedent) = graphe.BellmanFord(sommetDepart);

                Console.WriteLine($"\nDistances minimales depuis le sommet {sommetDepart} (Bellman-Ford) :");
                foreach (var kvp in distances)
                {
                    Console.WriteLine($"Vers {kvp.Key} : {kvp.Value}");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\nErreur : {ex.Message}");
            }
        }

        static void Interface()
        {
            bool continuer = true;
            while (continuer)
            {
                Titre();
                Console.WriteLine("\tMENU");
                Console.WriteLine("\nVeuillez vous connecter à votre compte:");
                Console.WriteLine("1: Connecter vous à un compte existant");
                Console.WriteLine("2: Créer un compte");
                Console.WriteLine("3: Quitter l'application\n");
                int idCompte;
                bool acces = false;
                int option = SaisieOption();
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Vous avez choisi de vous connecter à un compte existant");
                        Console.WriteLine("Veuillez entrer votre identifiant :");
                        idCompte = SaisNombre();
                        acces = Database.ConnexionCompte(idCompte);
                        ChoixCuisinierClient(idCompte, acces);
                        break;
                    case 2:
                        Console.WriteLine("Vous avez choisi de créer un compte");
                        idCompte = Database.CreationCompte();
                        acces = true;
                        ChoixCuisinierClient(idCompte, acces);
                        break;
                    case 3:
                        Console.WriteLine("Vous avez choisi de quitter l'application");
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Option invalide");
                        break;
                }
            }
        }

        static void ChoixCuisinierClient(int idCompte, bool acces)
        {
            Titre();
            if (!acces)
            {
                Console.WriteLine("Vous n'avez pas accès à votre compte");
                return;
            }
            while (acces)
            {
                Console.WriteLine("Voulez-vous consulter votre compte en tant que :\n\t1-Cuisinier \n\t2-Client \n\t3-Quitter");
                int option = SaisieOption();
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Vous avez choisi de consulter votre compte en tant que cuisinier");
                        Database.ConnexionCuisinier(idCompte);
                        break;
                    case 2:
                        Console.WriteLine("Vous avez choisi de consulter votre compte en tant que client");
                        Database.ConnexionClient(idCompte);
                        break;
                    case 3:
                        Console.WriteLine("Vous avez choisi de quitter ");
                        acces = false;
                        break;
                    default:
                        Console.WriteLine("Option invalide");
                        break;
                }
            }
        }

        public static void Titre()
        {
            Console.Clear();
            Console.WriteLine("Liv'in Paris\n");
        }

        static int SaisieOption()
        {
            Console.WriteLine("Veuillez choisir une des options proposées en saississant le numéro qu'il lui est associé");
            int nb;
            do
            {
                string result = Console.ReadLine();
                if (int.TryParse(result, out nb) && nb >= 0)
                {
                    return nb;
                }
                Console.WriteLine("Veuillez saisir un des chiffres proposés");
            } while (true);
        }

        public static int SaisNombre()
        {
            int nb;
            do
            {
                Console.WriteLine("Veuillez écrire un nombre strictement positif :");
                string result = Console.ReadLine();
                if (int.TryParse(result, out nb) && nb >= 1)
                {
                    return nb;
                }
                Console.WriteLine("Entrée invalide. Veuillez entrer un nombre strictement positif.");
            } while (true);
        }
    }
}
