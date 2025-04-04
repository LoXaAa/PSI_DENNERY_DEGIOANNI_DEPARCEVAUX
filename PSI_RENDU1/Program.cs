namespace PSI_RENDU1
{
    using System;
    using System.IO;
    using System.Data;
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
        Console.WriteLine("3: Voir la liste de compte");
        Console.WriteLine("4: Quitter l'application\n");
        int idCompte;
        bool acces = false;
        int option = SaisieOption();
        switch (option)
        {
            case 1:
                Console.WriteLine("Vous avez choisi de vous connecter à un compte existant");
                Console.WriteLine("Veuillez entrer votre identifiant :");
                idCompte = SaisNombre();
                acces= Database.ConnexionCompte(idCompte);
                ChoixCuisinierClient(idCompte, acces);
                break;
            case 2:
                Console.WriteLine("Vous avez choisi de créer un compte");
                idCompte=Database.AjouterCompte();
                acces = true;
                ChoixCuisinierClient(idCompte, acces);
                break;
            case 3:
                Titre();
                Database.MontrerEssentiel("Compte");
                break;
            case 4:
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
                ChoixCuisinier(idCompte);
                break;
            case 2:
                Console.WriteLine("Vous avez choisi de consulter votre compte en tant que client");
                Database.ConnexionClient(idCompte);
                ChoixClient(idCompte);
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
static void ChoixCuisinier(int idCompte)
{
    int idCuisinier = Database.RecupererId(idCompte, "Cuisinier");
    if (idCuisinier == -1)
    {
        Console.WriteLine("Aucun cuisinier associé à ce compte. Veuillez vérifier vos informations.");
        return;
    }
    bool continuer = true;
    while (continuer)
    {
        Titre();
        Console.WriteLine("Bienvenue dans votre espace Cuisinier\n");// à faire une fonction qui récupère le prenom du compte pour dire 'Bienvenue dans le menu cuisinier {toi}'
        Console.WriteLine("1: Gestion de vos recettes");
        Console.WriteLine("2: Gestion de vos plats");
        Console.WriteLine("3: Gestion de vos ingrédients");
        Console.WriteLine("4: Gestion de vos Commandes");//update valider la commande
        Console.WriteLine("5: Voir vos Avis");
        Console.WriteLine("6: Profil");// voir ses informations, peut les changer, voit cb il s'est fait, combien de plat il a préparé
        Console.WriteLine("7: Quitter");
        int option = SaisieOption();
        switch (option)
        {
            case 1:
                bool continuer1 = true;
                while (continuer1)
                {
                    Titre();
                    Console.WriteLine("Vous avez choisi de gérer vos recettes\n");
                    Console.WriteLine("1: Ajouter une recette");
                    Console.WriteLine("2: Supprimer une recette");
                    Console.WriteLine("3: Modifier une recette");
                    Console.WriteLine("4: Voir ses recettes");
                    Console.WriteLine("5: Quitter");
                    int option1 = SaisieOption();

                    switch (option1)
                    {
                        case 1:
                            Console.WriteLine("Vous avez choisi d'ajouter une recette");
                            Database.AjouterRecette();
                            break;
                        case 2:
                            Console.WriteLine("Vous avez choisi de supprimer une recette");
                            Database.Supprimer("Recette");
                            break;
                        case 3:
                            Console.WriteLine("Vous avez choisi de modifier une recette");
                            //Database.ModifierRecette(idCompte);
                            break;
                        case 4:
                            Console.WriteLine("Vous avez choisi de voir vos recettes");
                            Database.Montrer("Recette");
                            break;
                        case 5:
                            Console.WriteLine("Vous avez choisi de quitter");
                            continuer1 = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;
            case 2:
                bool continuer2 = true;
                while (continuer2)
                {
                    Titre();
                    Console.WriteLine("Vous avez choisi de gérer vos plats\n");
                    Console.WriteLine("1: Ajouter un plat");
                    Console.WriteLine("2: Supprimer un plat");
                    Console.WriteLine("3: Modifier un plat");
                    Console.WriteLine("4: Voir ses plats");
                    Console.WriteLine("5: Quitter");
                    int option2 = SaisieOption();
                    switch (option2)
                    {
                        case 1:
                            Console.WriteLine("Vous avez choisi d'ajouter un plat");
                            Database.AjouterPlat(idCompte);
                            break;
                        case 2:
                            Console.WriteLine("Vous avez choisi de supprimer un plat");
                            Database.Supprimer("Plat");
                            break;
                        case 3:
                            Console.WriteLine("Vous avez choisi de modifier un plat");
                            //Database.ModifierPlat(idCompte);
                            break;
                        case 4:
                            Console.WriteLine("Vous avez choisi de voir vos plats");
                            Database.Montrer("Plat",idCuisinier);
                            break;
                        case 5:
                            Console.WriteLine("Vous avez choisi de quitter");
                            continuer2 = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;
            case 3:
                bool continuer3 = true;
                while (continuer3)
                {
                    Titre();
                    Console.WriteLine("Vous avez choisi de gérer vos ingrédients\n");
                    Console.WriteLine("1: Ajouter un ingrédient");
                    Console.WriteLine("2: Supprimer un ingrédient");
                    Console.WriteLine("3: Modifier un ingrédient");
                    Console.WriteLine("4: Voir ses ingrédients");
                    Console.WriteLine("5: Quitter");
                    int option3 = SaisieOption();
                    switch (option3)
                    {
                        case 1:
                            Console.WriteLine("Vous avez choisi d'ajouter un ingrédient");
                            Database.AjouterIngredient();
                            break;
                        case 2:
                            Console.WriteLine("Vous avez choisi de supprimer un ingrédient");
                            Database.Supprimer("Ingredient");
                            break;
                        case 3:
                            Console.WriteLine("Vous avez choisi de modifier un ingrédient");
                            //Database.ModifierIngredient(idCompte);
                            break;
                        case 4:
                            Console.WriteLine("Vous avez choisi de voir vos ingrédients");
                            Database.Montrer("Ingredient");
                            break;
                        case 5:
                            Console.WriteLine("Vous avez choisi de quitter");
                            continuer3 = false;
                            break;
                    }

                }
                break;
            case 4:
                bool continuer4 = true;
                while (continuer4)
                {
                    Titre();
                    Console.WriteLine("Vous avez choisi de gérer vos commandes\n");
                    Console.WriteLine("2: Supprimer une commande");
                    Console.WriteLine("3: Modifier une commande");
                    Console.WriteLine("4: Voir ses commandes");
                    Console.WriteLine("5: Quitter");
                    int option4 = SaisieOption();
                    switch (option4)
                    {
                        case 2:
                            Console.WriteLine("Vous avez choisi de supprimer une commande");
                            Database.Supprimer("Commande");
                            break;
                        case 3:
                            Console.WriteLine("Vous avez choisi de modifier une commande");
                            //Database.ModifierCommande(idCompte);
                            break;
                        case 4:
                            Console.WriteLine("Vous avez choisi de voir vos commandes");
                            Database.Montrer("Commande",idCuisinier);
                            break;
                        case 5:
                            Console.WriteLine("Vous avez choisi de quitter");
                            continuer4 = false;
                            break;
                        default:
                            Console.WriteLine("Option invalide");
                            break;
                    }
                }
                break;// à changer
            case 5:
                Console.WriteLine("Vous avez choisi de voir vos avis");
                Database.Montrer("Avis",idCuisinier); //voir si ne pas faire une fonction pour montter les avis
                break;
            case 6:
                Console.WriteLine("Vous avez choisi de voir votre profil");
                Database.MontrerProfilCuisinier(idCuisinier); 
                break;
            case 7:
                Console.WriteLine("Vous avez choisi de quitter");
                continuer = false;
                break;
            default:
                Console.WriteLine("Option invalide");
                break;
        }
    }
}
static void ChoixClient(int idCompte)
{
    int idClient = Database.RecupererId(idCompte, "Client");
    if (idClient == -1)
    {
        Console.WriteLine("Aucun cuisinier associé à ce compte. Veuillez vérifier vos informations.");
        return;
    }
    bool continuer = true;
    while (continuer)
    {
        Titre();
        Console.WriteLine("Bienvenue dans votre espace Client\n");// à faire une fonction qui récupère le prenom du compte pour dire 'Bienvenue dans le menu cuisinier {toi}'
        Console.WriteLine("1: Commander");// voir les commandes possibles
        Console.WriteLine("2: Voir vos commandes"); //en cours/terminée
        Console.WriteLine("3: Faites un avis sur un plat");
        Console.WriteLine("4: Profil");
        Console.WriteLine("5: Quitter");
        int option = SaisieOption();
        switch (option)
        {
            case 1:
                Console.WriteLine("Vous avez choisi de commander");
                Database.Commander(idClient);
                break;
            case 2:
                Console.WriteLine("Vous avez choisi de voir vos commandes");
                Database.Montrer("Commande");
                break;
            case 3:
                Console.WriteLine("Vous avez choisi de faire un avis sur un plat");
                Database.AjouterAvis(idClient,idCompte);//faux
                break;
            case 4:
                Console.WriteLine("Vous avez choisi de voir votre profil");
                Database.MontrerProfilClient(idClient); //voir si ne pas faire une fonction pour montter les avis
                break;
            case 5:
                Console.WriteLine("Vous avez choisi de quitter");
                continuer = false;
                break;
            default:
                Console.WriteLine("Option invalide");
                break;
        }
    }

}
public static void Titre()
{
    //Console.Clear();
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
