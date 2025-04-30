namespace PSI_RENDU1
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using PSI_RENDU1;

    internal class Program
    {
#region Main
        public static void Main(string[] args)
        {
            Database.InitDatabase();
            bool continuer = true;
            while (continuer)
            {
                Titre();
                Console.WriteLine("1. Module Graphes");
                Console.WriteLine("2. Module Clients / Cuisiniers");
                Console.WriteLine("3. Quitter");
                Console.Write("Choix : ");
                string choixModule = Console.ReadLine();

                switch (choixModule)
                {
                    case "1":
                        ModuleGraphes();
                        break;
                    case "2":
                        Interface();
                        break;
                    case "3":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        break;
                }
            }
        }

        public static void ModuleGraphes()
        {
            var graphe = new Graphe<int>();

            Console.WriteLine("1 --> Charger le métro | 2 --> Graphe aléatoire");
            Console.Write("Choix : ");
            string choix = Console.ReadLine();
            if (choix == "1")
            {
                string fichier = "MetroParis_A.xlsx";
                if (!File.Exists(fichier))
                {
                    Console.WriteLine("Fichier introuvable");
                    return;
                }
                var donneesNoeuds = LecteurExcel.LireNoeuds(fichier);
                foreach (var (id, nom, lon, lat, ligne) in donneesNoeuds)
                    graphe.AjouterNoeud(id, nom, lon, lat, ligne);
                var doublons = donneesNoeuds.GroupBy(n => n.nom).Where(g => g.Count() > 1);
                foreach (var g in doublons)
                {
                    var liste = g.Select(n => n.id).ToList();
                    for (int i = 0; i < liste.Count; i++)
                        for (int j = i + 1; j < liste.Count; j++)
                        {
                            graphe.AjouterLien(liste[i], liste[j], 0.5);
                            graphe.AjouterLien(liste[j], liste[i], 0.5);
                        }
                }
                var arcs = LecteurExcel.LireArcs(fichier);
                foreach (var (src, dst, pds, su) in arcs)
                {
                    graphe.AjouterLien(src, dst, pds);
                    if (!su)
                        graphe.AjouterLien(dst, src, pds);
                }
                Console.WriteLine("Graphe chargé avec succès");
            }
            else if (choix == "2")
            {
                Console.Write("Nombre de sommets : ");
                int n = int.Parse(Console.ReadLine());
                Console.Write("Nombre de liens : ");
                int m = int.Parse(Console.ReadLine());
                graphe.GenererGrapheAleatoire(n, m);
            }
            else
            {
                Console.WriteLine("Choix invalide");
                return;
            }
            Console.Write("Parcours en longueur --> 1 | Parcours en largeur --> 2 : ");
            string cp = Console.ReadLine();
            Console.Write("Sommet de départ : ");
            int sd = int.Parse(Console.ReadLine());
            if (cp == "1")
                graphe.ParcoursProfondeur(sd);
            else if (cp == "2")
                graphe.ParcoursLargeur(sd);

            Console.WriteLine(graphe.EstConnexe() ? "Connexité" : "Non connexe");
            Console.WriteLine(graphe.ContientUnCycle() ? "Présence de cycle" : "Acyclique");
            GrapheVisualisation.GenererImageGraphe(graphe, "graphe_esthetique.png");

            Console.Write("Algo de plus court chemin : Dijkstra --> 1 | Bellman-Ford --> 2 | Floyd-Warshall --> 3:");
            string ca = Console.ReadLine();
            if (ca == "1")
                ExecuterDijkstra(graphe);
            else if (ca == "2")
                ExecuterBellmanFord(graphe);
            else if (ca == "3")
                ExecuterFloydWarshall(graphe);

            Console.WriteLine("Coloration en cours...");
            int nbC = GraphColoring<int>.WelshPowell(graphe);
            Console.WriteLine($"Graphe colorié avec {nbC} couleurs");
            GrapheVisualisation.GenererImageGraphe(graphe, "graphe_clusters.png", 2000, 1400, true);

            var groupes = graphe.ObtenirGroupesIndependants();
            foreach (var kv in groupes.OrderBy(kv => kv.Key))
                Console.WriteLine($"Groupe {kv.Key} : {string.Join(", ", kv.Value)}");

            Console.WriteLine($"Biparti ? --> {graphe.EstBiparti()}");
            Console.WriteLine($"Planaire ? --> {graphe.EstPlanaire()}");

            GraphExporter<int>.ExporterEnJson("noeuds.json", graphe);
            GraphExporter<int>.ExporterEnXml("noeuds.xml", graphe);
            Console.WriteLine("Exports JSON/XML réalisés avec succès");

            var grapheRel = ConstructeurGrapheRelations.ConstruireDepuisBDD();


            Console.Write("ID client : ");
            int idClient = int.Parse(Console.ReadLine()!);
            Console.Write("ID cuisinier : ");
            int idCuisinier = int.Parse(Console.ReadLine()!);
            var chemin = grapheRel.PlusCourtChemin($"C{idClient}", $"U{idCuisinier}");
            if (chemin.Count == 0)
                Console.WriteLine("Aucun chemin trouvé.");
            else
                Console.WriteLine("Plus court chemin : " + string.Join(" → ", chemin));
        }
#endregion

#region Interface

        /// <summary>
        /// Menu de gestion des clients, cuisiniers et commandes.
        /// </summary>

        static void Interface()
        {
            bool continuer = true;
            while (continuer)
            {
                Titre();
                Console.WriteLine("1: Se connecter");
                Console.WriteLine("2: Créer un compte");
                Console.WriteLine("3: Voir la liste des comptes");
                Console.WriteLine("4: Quitter");
                int option = SaisieOption();
                int idCompte;
                bool acces = false;
                switch (option)
                {
                    case 1:
                        Console.Write("Identifiant : ");
                        idCompte = SaisNombre();
                        acces = Database.ConnexionCompte(idCompte);
                        ChoixCuisinierClient(idCompte, acces);
                        break;
                    case 2:
                        idCompte = Database.AjouterCompte();
                        acces = true;
                        ChoixCuisinierClient(idCompte, acces);
                        break;
                    case 3:
                        Database.MontrerEssentiel("Compte");
                        break;
                    case 4:
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Option invalide");
                        break;
                }
            }
        }

        /// <summary>
        /// Algorithme Dijkstra
        /// </summary>
        static void ExecuterDijkstra<T>(Graphe<T> graphe)
        {
            Console.Write("ID départ : ");
            string input = Console.ReadLine();
            T depart = (T)Convert.ChangeType(input, typeof(T));
            var (distances, precedent) = graphe.Dijkstra(depart);
            Console.WriteLine($"Distances depuis {depart} (Dijkstra) :");
            foreach (var kv in distances)
                Console.WriteLine($"Vers {kv.Key} : {kv.Value}");
        }

        /// <summary>
        /// Algorithme Bellman-Ford
        /// </summary>
        static void ExecuterBellmanFord<T>(Graphe<T> graphe)
        {
            Console.Write("ID départ : ");
            string input = Console.ReadLine();
            T depart = (T)Convert.ChangeType(input, typeof(T));
            try
            {
                var (distances, precedent) = graphe.BellmanFord(depart);
                Console.WriteLine($"Distances depuis {depart} (Bellman-Ford) :");
                foreach (var kv in distances)
                    Console.WriteLine($"Vers {kv.Key} : {kv.Value}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }

        /// <summary>
        /// Algorithme Floyd-Warshall
        /// </summary>
        static void ExecuterFloydWarshall(Graphe<int> graphe)
        {
            var (distances, precedent) = graphe.FloydWarshall();
            Console.Write("ID départ : ");
            if (!int.TryParse(Console.ReadLine(), out int depart) 
                || !graphe.Noeuds.ContainsKey(depart))
            {
                Console.WriteLine("Entrée invalide");
                return;
            }
            Console.WriteLine($"Distances depuis {depart} (Floyd-Warshall) :");
            foreach (var dst in graphe.Noeuds.Keys.OrderBy(k => k))
            {
                double d = distances[depart][dst];
                Console.WriteLine($"Vers {dst} : {(double.IsInfinity(d) ? "∞" : d.ToString())}");
            }
        }

        /// <summary>
        /// Menu Cuisinier/Client après connexion
        /// </summary>
        static void ChoixCuisinierClient(int idCompte, bool acces)
        {
            if (!acces)
            {
                Console.WriteLine("Accès refusé");
                return;
            }
            while (true)
            {
                Console.WriteLine("1: Cuisinier   2: Client   3: Quitter");
                int choix = SaisieOption();
                switch (choix)
                {
                    case 1:
                        Database.ConnexionCuisinier(idCompte);
                        ChoixCuisinier(idCompte);
                        break;
                    case 2:
                        Database.ConnexionClient(idCompte);
                        ChoixClient(idCompte);
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Option invalide");
                        break;
                }
            }
        }

        /// <summary>
        /// Menu spécifique au cuisinier
        /// </summary>
        static void ChoixCuisinier(int idCompte)
{
    int idCuisinier = Database.RecupererId(idCompte, "Cuisinier");
    if (idCuisinier < 1)
    {
        Console.WriteLine("Aucun cuisinier associé");
        return;
    }

    bool continuer = true;
    while (continuer)
    {
        Titre();
        Console.WriteLine("1: Gestion de vos recettes");
        Console.WriteLine("2: Gestion de vos plats");
        Console.WriteLine("3: Gestion de vos ingrédients");
        Console.WriteLine("4: Gestion de vos commandes");
        Console.WriteLine("5: Voir vos avis");
        Console.WriteLine("6: Profil");
        Console.WriteLine("7: Quitter");
        int choix = SaisieOption();

        switch (choix)
        {
            case 1:
                // Recettes
                bool contRec = true;
                while (contRec)
                {
                    Titre();
                    Console.WriteLine("Recettes : 1-Ajouter 2-Supprimer 3-Modifier 4-Voir 5-Quitter");
                    int op = SaisieOption();
                    switch (op)
                    {
                        case 1:
                            Database.AjouterRecette();
                            break;
                        case 2:
                            Database.Supprimer("Recette");
                            break;
                        case 3:
                            Database.ModifierRecette();
                            break;
                        case 4:
                            Database.Montrer("Recette");
                            break;
                        case 5:
                            contRec = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;

            case 2:
                bool contPlat = true;
                while (contPlat)
                {
                    Titre();
                    Console.WriteLine("Plats : 1-Ajouter 2-Supprimer 3-Modifier 4-Voir 5-Quitter");
                    int op2 = SaisieOption();
                    switch (op2)
                    {
                        case 1:
                            Database.AjouterPlat(idCompte);
                            break;
                        case 2:
                            Database.Supprimer("Plat");
                            break;
                        case 3:
                            Database.ModifierPlat(idCompte);
                            break;
                        case 4:
                            Database.Montrer("Plat", Database.RecupererId(idCompte, "Cuisinier"));
                            break;
                        case 5:
                            contPlat = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;

            case 3:
                bool contIng = true;
                while (contIng)
                {
                    Titre();
                    Console.WriteLine("Ingrédients : 1-Ajouter 2-Supprimer 3-Modifier 4-Voir 5-Quitter");
                    int op3 = SaisieOption();
                    switch (op3)
                    {
                        case 1:
                            Database.AjouterIngredient();
                            break;
                        case 2:
                            Database.Supprimer("Ingredient");
                            break;
                        case 3:
                            Database.ModifierIngredient();
                            break;
                        case 4:
                            Database.Montrer("Ingredient");
                            break;
                        case 5:
                            contIng = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;

            case 4:
                bool contCmd = true;
                while (contCmd)
                {
                    Titre();
                    Console.WriteLine("Commandes : 1 --> Ajouter | 2 --> Supprimer | 3 --> Modifier | 4 --> Voir | 5 --> Quitter");
                    int op4 = SaisieOption();
                    switch (op4)
                    {
                        case 1:
                            Console.WriteLine("\n🔎 Commandes en attente pour ce cuisinier :");
                            Database.Montrer("Commande", Database.RecupererId(idCompte, "Cuisinier"));

                            Console.Write("\nID de la commande à valider : ");
                            int idCmd;
                            while (!int.TryParse(Console.ReadLine(), out idCmd))
                                Console.Write("Veuillez entrer un ID valide : ");

                            Console.Write("Nouveau statut : ");
                            string nouveauStatut = Console.ReadLine()?.Trim() ?? "";

                            Database.ModifierCommande(idCompte);
                            break;
                        case 2:
                            Database.Supprimer("Commande");
                            break;
                        case 3:
                            Database.ModifierCommande(idCompte);
                            break;
                        case 4:
                            Database.Montrer("Commande", Database.RecupererId(idCompte, "Cuisinier"));
                            break;
                        case 5:
                            contCmd = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;

            case 5:
                Database.Montrer("Avis", idCuisinier);
                break;

            case 6:
                Database.MontrerProfilCuisinier(idCuisinier);
                break;

            case 7:
                continuer = false;
                break;

            default:
                Console.WriteLine("Option invalide");
                break;
        }
    }
}

        /// <summary>
        /// Menu specifique au client
        /// </summary>
        static void ChoixClient(int idCompte)
{
    int idClient = Database.RecupererId(idCompte, "Client");
    if (idClient < 1)
    {
        Console.WriteLine("Aucun client associé");
        return;
    }

    bool continuer = true;
    while (continuer)
    {
        Titre();
        Console.WriteLine("1: Commander");
        Console.WriteLine("2: Voir vos commandes");
        Console.WriteLine("3: Faire un avis");
        Console.WriteLine("4: Profil");
        Console.WriteLine("5: Quitter");
        int choix = SaisieOption();

        switch (choix)
        {
            case 1:
                Database.Commander(idClient);
                break;
            case 2:
                Database.Montrer("Commande", idClient);
                break;
            case 3:
                Database.AjouterAvis(idClient, idCompte);
                break;
            case 4:
                Database.MontrerProfilClient(idClient);
                break;
            case 5:
                continuer = false;
                break;
            default:
                Console.WriteLine("Option invalide");
                break;
        }
    }
}

        /// <summary>
        /// Affiche le titre de l'application
        /// </summary>
        public static void Titre()
        {
            Console.WriteLine("Liv'in Paris\n");
        }

        /// <summary>
        /// Lit une option dans la console
        /// </summary>
        static int SaisieOption()
        {
            while (true)
            {
                string s = Console.ReadLine();
                if (int.TryParse(s, out int v) && v >= 1)
                    return v;
                Console.WriteLine("Entrée invalide");
            }
        }

        /// <summary>
        /// Lecture nombre
        /// </summary>
        public static int SaisNombre()
        {
            while (true)
            {
                string s = Console.ReadLine();
                if (int.TryParse(s, out int v) && v > 0)
                    return v;
                Console.WriteLine("Réessayez");
            }
        }
#endregion
    }
}
