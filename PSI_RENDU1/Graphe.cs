using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace PSI_RENDU1
{

    public class Graphe
    {
        private Dictionary<int, Noeud> noeuds;
        private List<Lien> liens;

        public Graphe()
        {
            noeuds = new Dictionary<int, Noeud>();
            liens = new List<Lien>();
        }

        public void AjouterLien(int idSource, int idDestination, double poids)
        {
            if (!noeuds.ContainsKey(idSource))
                noeuds[idSource] = new Noeud(idSource);

            if (!noeuds.ContainsKey(idDestination))
                noeuds[idDestination] = new Noeud(idDestination);

            var lien = new Lien(noeuds[idSource], noeuds[idDestination], poids);
            noeuds[idSource].Liens.Add(lien);
            noeuds[idDestination].Liens.Add(lien);
            liens.Add(lien);
        }

        public void ChargerDepuisCSV(string cheminFichier)
        {
            Console.WriteLine($"Lecture du fichier CSV : {cheminFichier}");

            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Erreur : Fichier introuvable !");
                return;
            }

            using (var reader = new StreamReader(cheminFichier))
            {
                string ligne;
                bool premiereLigne = true;

                while ((ligne = reader.ReadLine()) != null)
                {
                    if (premiereLigne) // Ignorer l'en-tête
                    {
                        premiereLigne = false;
                        continue;
                    }

                    string[] champs = ligne.Split(';'); // Modifier si séparateur différent (ex: `,`)

                    if (champs.Length < 8) continue; // Vérification du bon format

                    int idStation = int.Parse(champs[0]);
                    string nomStation = champs[2];
                    double longitude = double.Parse(champs[3], CultureInfo.InvariantCulture);
                    double latitude = double.Parse(champs[4], CultureInfo.InvariantCulture);
                    int tempsTrajet = string.IsNullOrWhiteSpace(champs[7]) ? 0 : int.Parse(champs[7]);

                    if (!noeuds.ContainsKey(idStation))
                        noeuds[idStation] = new Noeud(idStation, nomStation, longitude, latitude);

                    if (tempsTrajet > 0 && idStation > 1)
                    {
                        int idStationPrecedente = idStation - 1;
                        AjouterLien(idStationPrecedente, idStation, tempsTrajet);
                    }
                }
            }
        }

        public void AfficherGraphe()
        {
            foreach (var noeud in noeuds.Values)
            {
                Console.Write($"Station {noeud.Id} ({noeud.Nom}) : ");
                foreach (var lien in noeud.Liens)
                {
                    Console.Write($" -> {lien.Destination.Nom} (Temps: {lien.Poids} min) ");
                }
                Console.WriteLine();
            }
        }
    

        #region Chargement Fichier
        /// <summary>
        /// Lecture du fichier
        /// </summary>
        /// <param name="cheminFichier">Chemin relatif du fichier cible</param>
        public void ChargerDepuisFichier(string cheminFichier)
        {
            Console.WriteLine($"Lecture du fichier : {cheminFichier}");

            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Erreur : Fichier introuvable !");
                return;
            }

            foreach (var ligne in File.ReadAllLines(cheminFichier))
            {
                string cleanLigne = ligne.Trim();
                if (string.IsNullOrWhiteSpace(cleanLigne)) continue; 

                var match = Regex.Match(cleanLigne, @"\((\d+),\s*(\d+)\)\s+([\d.]+)");
                if (match.Success)
                {
                    int idSource = int.Parse(match.Groups[1].Value);
                    int idDestination = int.Parse(match.Groups[2].Value);
                    double poids = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    AjouterLien(idSource, idDestination, poids);
                }
                else
                {
                    Console.WriteLine($"⚠ Erreur de parsing : {cleanLigne}");
                }
            }
        }
        #endregion
        #region Génération d'un graphe aléatoire
        /// <summary>
        /// Génération aléatoire d'un graphe
        /// </summary>
        /// <param name="nombreSommets">Nombre de sommets souhaités</param>
        /// <param name="nombreLiens">Nombre de liens souhaités</param>
        /// <param name="estPondere">graphe pondéré ou non (non par defaut)</param>
        public void GenererGrapheAleatoire(int nombreSommets, int nombreLiens, bool estPondere = false)
        {
            noeuds.Clear();
            liens.Clear();

            for (int i = 0; i < nombreSommets; i++)
            {
                noeuds[i] = new Noeud(i);
            }

            HashSet<(int, int)> liensAjoutes = new HashSet<(int, int)>();

            Random random = new Random();


            while (liensAjoutes.Count < nombreLiens)
            {
                int source = random.Next(nombreSommets);
                int destination = random.Next(nombreSommets);

                if (source != destination && !liensAjoutes.Contains((source, destination)) && !liensAjoutes.Contains((destination, source)))
                {
                    double poids = estPondere ? random.NextDouble() * 10 : 1.0;
                    AjouterLien(source, destination, poids);
                    liensAjoutes.Add((source, destination));
                }
            }
        }
        #endregion
        #region Construction Matrice Adjacence
        /// <summary>
        /// Construction de la matrice d'adjacence du graphe
        /// </summary>
        public void ConstruireMatriceAdjacence()
        {
            int maxId = 0;
            foreach (var noeud in noeuds.Keys)
            {
                if (noeud > maxId)
                    maxId = noeud;
            }

            int taille = maxId + 1;
            double[,] matrice = new double[taille, taille];

            foreach (var lien in liens)
            {
                int i = lien.Source.Id;
                int j = lien.Destination.Id;
                matrice[i, j] = lien.Poids;
                matrice[j, i] = lien.Poids;
            }

            // Affichage formaté de la matrice
            Console.WriteLine("\nMatrice d'Adjacence :");

            Console.Write("    "); 
            for (int i = 0; i < taille; i++)
            {
                Console.Write($"{i,3} "); 
            }
            Console.WriteLine();


            Console.Write("   ");
            Console.WriteLine(new string('-', 4 * taille));


            for (int i = 0; i < taille; i++)
            {
                Console.Write($"{i,2} |");
                for (int j = 0; j < taille; j++)
                {
                    if (matrice[i, j] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"  1 ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"  0 ");
                    }
                }
                Console.WriteLine();
            }



        }
        #endregion
        #region Parcours en Profondeur
        /// <summary>
        /// Parcours en profondeur du graphe
        /// </summary>
        /// <param name="depart">noeud de départ</param>
        public void ParcoursProfondeur(int depart)
        {
            HashSet<int> visites = new HashSet<int>();
            Stack<int> pile = new Stack<int>();

            pile.Push(depart);

            Console.WriteLine("Parcours en profondeur (DFS) :");

            while (pile.Count > 0)
            {
                int noeudActuel = pile.Pop();

                if (!visites.Contains(noeudActuel))
                {
                    Console.Write(noeudActuel + " ");
                    visites.Add(noeudActuel);

                    foreach (var voisin in noeuds[noeudActuel].Liens.Select(l => l.Destination.Id))
                    {
                        if (!visites.Contains(voisin))
                        {
                            pile.Push(voisin);
                        }
                    }
                }
            }
            Console.WriteLine();
        }
        #endregion
        #region Parcorus en Largeur
        /// <summary>
        /// Parcours en largeur du graphe
        /// </summary>
        /// <param name="depart">noeud de départ</param>
        public void ParcoursLargeur(int depart)
        {
            HashSet<int> visites = new HashSet<int>();
            Queue<int> file = new Queue<int>();

            file.Enqueue(depart);
            visites.Add(depart);

            Console.WriteLine("Parcours en largeur (BFS) :");

            while (file.Count > 0)
            {
                int noeudActuel = file.Dequeue();
                Console.Write(noeudActuel + " ");

                foreach (var voisin in noeuds[noeudActuel].Liens.Select(l => l.Destination.Id))
                {
                    if (!visites.Contains(voisin))
                    {
                        visites.Add(voisin);
                        file.Enqueue(voisin);
                    }
                }
            }
            Console.WriteLine();
        }
        #endregion
        #region Detection connexité
        /// <summary>
        /// Detection de la connexité d'un graphe
        /// </summary>
        /// <returns>True ou False</returns>
        public bool EstConnexe()
        {
            if (noeuds.Count == 0) return false; // Un graphe vide n'est pas connexe

            HashSet<int> visités = new HashSet<int>();
            Queue<Noeud> file = new Queue<Noeud>();

            // Prendre un sommet quelconque pour commencer (premier de la liste)
            var premierNoeud = noeuds.Values.First();
            file.Enqueue(premierNoeud);
            visités.Add(premierNoeud.Id);

            while (file.Count > 0)
            {
                Noeud courant = file.Dequeue();
                foreach (var lien in courant.Liens)
                {
                    Noeud voisin = lien.Destination;
                    if (!visités.Contains(voisin.Id))
                    {
                        visités.Add(voisin.Id);
                        file.Enqueue(voisin);
                    }
                }
            }

            return visités.Count == noeuds.Count; // Si on a visité tous les sommets, le graphe est connexe
        }
        #endregion
        #region Detection Cycle
        /// <summary>
        /// Bolléen d'affichage si le graphe contient un cycle
        /// </summary>
        /// <returns>True ou False</returns>
        public bool ContientUnCycle()
        {
            HashSet<int> visités = new HashSet<int>();

            foreach (var noeud in noeuds.Values)
            {
                if (!visités.Contains(noeud.Id))
                {
                    if (DFS_DetectCycle(noeud, visités, null)) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// boucle qui detecte le cycle
        /// </summary>
        /// <param name="courant">Le nœud actuellement exploré dans le parcours en profondeur</param>
        /// <param name="visités">Ensemble des identifiants des nœuds déjà visités pour éviter les boucles infinies</param>
        /// <param name="parent">Le nœud par lequel on est arrivé au nœud courant</param>
        /// <returns></returns>
        private bool DFS_DetectCycle(Noeud courant, HashSet<int> visités, Noeud parent)
        {
            visités.Add(courant.Id);

            foreach (var lien in courant.Liens)
            {
                Noeud voisin = lien.Destination;

                // Si voisin n'a pas encore été visité, continuer le DFS
                if (!visités.Contains(voisin.Id))
                {
                    if (DFS_DetectCycle(voisin, visités, courant)) return true;
                }
                // Si voisin a déjà été visité et n'est pas le parent immédiat -> cycle détecté
                else if (voisin != parent)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion


        #region Dijkstra
        public (Dictionary<int, double> distances, Dictionary<int, int?> precedent) Dijkstra(int source)
        {
            var distances = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();
            var priorityQueue = new SortedSet<(double distance, int noeud)>();

            // Initialisation des distances à l'infini
            foreach (var noeud in noeuds.Keys)
            {
                distances[noeud] = double.PositiveInfinity;
                precedent[noeud] = null;
            }

            // La distance au point de départ est de 0
            distances[source] = 0;
            priorityQueue.Add((0, source));

            while (priorityQueue.Count > 0)
            {
                // Extraction du nœud avec la plus petite distance
                var (currentDistance, currentNode) = priorityQueue.Min;
                priorityQueue.Remove(priorityQueue.Min);

                foreach (var lien in noeuds[currentNode].Liens)
                {
                    int voisin = lien.Destination.Id;
                    double nouvelleDistance = currentDistance + lien.Poids;

                    if (nouvelleDistance < distances[voisin])
                    {
                        // Mise à jour de la distance minimale
                        priorityQueue.Remove((distances[voisin], voisin));
                        distances[voisin] = nouvelleDistance;
                        precedent[voisin] = currentNode;
                        priorityQueue.Add((nouvelleDistance, voisin));
                    }
                }
            }

            return (distances, precedent);
        }


        #endregion

        #region Bellman-Ford
        public (Dictionary<int, double> distances, Dictionary<int, int?> precedent) BellmanFord(int source)
        {
            var distances = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();

            // Initialisation : distance infinie pour tous sauf le sommet source
            foreach (var noeud in noeuds.Keys)
            {
                distances[noeud] = double.PositiveInfinity;
                precedent[noeud] = null;
            }
            distances[source] = 0;

            int nombreSommets = noeuds.Count;

            // Relaxation des arêtes (nombreSommets - 1) fois
            for (int i = 0; i < nombreSommets - 1; i++)
            {
                foreach (var noeud in noeuds.Values)
                {
                    foreach (var lien in noeud.Liens)
                    {
                        int u = noeud.Id;
                        int v = lien.Destination.Id;
                        double poids = lien.Poids;

                        if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                        {
                            distances[v] = distances[u] + poids;
                            precedent[v] = u;
                        }
                    }
                }
            }

            // Vérification de la présence d'un cycle de poids négatif
            foreach (var noeud in noeuds.Values)
            {
                foreach (var lien in noeud.Liens)
                {
                    int u = noeud.Id;
                    int v = lien.Destination.Id;
                    double poids = lien.Poids;

                    if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                    {
                        throw new InvalidOperationException("Le graphe contient un cycle de poids négatif.");
                    }
                }
            }

            return (distances, precedent);
        }
        #endregion


    }
}
