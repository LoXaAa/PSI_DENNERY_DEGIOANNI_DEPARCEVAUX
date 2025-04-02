namespace PSI_RENDU1
{
    using System;
    using System.IO;
    using PSI_RENDU1;


    internal class Program
    {
        static void Main()
        {


            Graphe graphe = new Graphe();

            Console.WriteLine("Voulez-vous :\n1. Charger le graphe karaté\n2. Générer un graphe aléatoire ?");
            Console.Write("Entrez 1 ou 2 : ");
            string choix = Console.ReadLine();

            if (choix == "1")
            {
                string cheminFichier = Path.Combine(Directory.GetCurrentDirectory(), "soc-karate.txt");

                if (!File.Exists(cheminFichier))
                {
                    Console.WriteLine("Fichier introuvable");
                    return;
                }

                graphe.ChargerDepuisFichier(cheminFichier);
            }
            else if (choix == "2")
            {
                Console.Write("Combien de sommets pour le graphe ?");
                int nombreSommets = int.Parse(Console.ReadLine());

                Console.Write("Combien d'arrêtes pour le graphe ?");
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
            {
                graphe.ParcoursProfondeur(sommetDepart);
            }
            else if (choixParcours == "2")
            {
                graphe.ParcoursLargeur(sommetDepart);
            }
            else
            {
                Console.WriteLine("Choix invalide");
            }

            if (graphe.EstConnexe())
            {
                Console.WriteLine("Le graphe est connexe");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe");
            }
            if (graphe.ContientUnCycle())
            {
                Console.WriteLine("Le graphe contient un ou plusieurs cycles");
            }
            else
            {
                Console.WriteLine("Le graphe est acyclique");
            }

            Console.WriteLine("\nAffichage du graphe :");
            string imagePath = "graphe.png";
            GrapheVisualisation.GenererImageGraphe(graphe, imagePath);

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
