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
    }
}
