using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using ClosedXML.Excel;

namespace PSI_RENDU1
{

    public class Graphe<T>
    {
        private Dictionary<T, Noeud<T>> noeuds;
        private List<Lien<T>> liens;



        public Graphe()
        {
            noeuds = new Dictionary<T, Noeud<T>>();
            liens = new List<Lien<T>>();
        }
        public Dictionary<T, Noeud<T>> Noeuds => noeuds;
        public List<Lien<T>> Liens => liens;

        public void AjouterNoeud(T id, string nom = "", double longitude = 0, double latitude = 0)
        {
            if (!noeuds.ContainsKey(id))
                noeuds[id] = new Noeud<T>(id, nom, longitude, latitude);
        }


        public void AjouterLien(T idSource, T idDestination, double poids)
{
    if (!noeuds.ContainsKey(idSource) || !noeuds.ContainsKey(idDestination))
    {
        Console.WriteLine($"⚠️ Lien ignoré : {idSource} → {idDestination} (station manquante)");
        return;
    }

    var source = noeuds[idSource];
    var destination = noeuds[idDestination];

    // Vérifier si le lien existe déjà pour éviter les doublons
    if (source.Liens.Any(l => l.Destination.Id.Equals(destination.Id)))
        return;

    var lien = new Lien<T>(source, destination, poids);
    source.Liens.Add(lien);
    liens.Add(lien);
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
        T id = (T)Convert.ChangeType(i, typeof(T));
        AjouterNoeud(id);
    }

    var liensAjoutes = new HashSet<(T, T)>();
    var random = new Random();

    while (liens.Count < nombreLiens)
    {
        int src = random.Next(nombreSommets);
        int dst = random.Next(nombreSommets);
        if (src == dst) continue;

        T source = (T)Convert.ChangeType(src, typeof(T));
        T destination = (T)Convert.ChangeType(dst, typeof(T));

        if (!liensAjoutes.Contains((source, destination)) && !liensAjoutes.Contains((destination, source)))
        {
            double poids = estPondere ? random.NextDouble() * 10 : 1;
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
        List<T> ids = noeuds.Keys.ToList();
        int taille = ids.Count;
        double[,] matrice = new double[taille, taille];

        for (int k = 0; k < liens.Count; k++)
        {
            int i = ids.IndexOf(liens[k].Source.Id);
            int j = ids.IndexOf(liens[k].Destination.Id);
            matrice[i, j] = liens[k].Poids;
            matrice[j, i] = liens[k].Poids;
        }

        Console.WriteLine("\nMatrice d'Adjacence :");
        Console.Write("    ");
        for (int i = 0; i < taille; i++) Console.Write($"{ids[i],3} ");
        Console.WriteLine();
        Console.Write("   ");
        Console.WriteLine(new string('-', 4 * taille));

        for (int i = 0; i < taille; i++)
        {
            Console.Write($"{ids[i],2} | ");
            for (int j = 0; j < taille; j++)
            {
                if (matrice[i, j] == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("  1 ");
                    Console.ResetColor();
                }
                else Console.Write("  0 ");
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
        public void ParcoursProfondeur(T depart)
    {
        HashSet<T> visites = new HashSet<T>();
        Stack<T> pile = new Stack<T>();

        pile.Push(depart);
        Console.WriteLine("Parcours en profondeur (DFS) :");

        while (pile.Count > 0)
        {
            T noeudActuel = pile.Pop();

            if (!visites.Contains(noeudActuel))
            {
                Console.Write(noeudActuel + " ");
                visites.Add(noeudActuel);

                foreach (var voisin in noeuds[noeudActuel].Liens.Select(l => l.Destination.Id))
                {
                    if (!visites.Contains(voisin))
                        pile.Push(voisin);
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
        public void ParcoursLargeur(T depart)
{
    HashSet<T> visites = new HashSet<T>();
    Queue<T> file = new Queue<T>();

    file.Enqueue(depart);
    visites.Add(depart);

    Console.WriteLine("Parcours en largeur (BFS) :");

    while (file.Count > 0)
    {
        T noeudActuel = file.Dequeue();
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

    HashSet<T> visites = new HashSet<T>();
    Queue<Noeud<T>> file = new Queue<Noeud<T>>();

    var premierNoeud = noeuds.Values.First();
    file.Enqueue(premierNoeud);
    visites.Add(premierNoeud.Id);

    while (file.Count > 0)
    {
        Noeud<T> courant = file.Dequeue();
        foreach (var lien in courant.Liens)
        {
            Noeud<T> voisin = lien.Destination;
            if (!visites.Contains(voisin.Id))
            {
                visites.Add(voisin.Id);
                file.Enqueue(voisin);
            }
        }
    }

    return visites.Count == noeuds.Count; // Si on a visité tous les sommets, le graphe est connexe
}
        #endregion
        #region Detection Cycle
        /// <summary>
        /// Bolléen d'affichage si le graphe contient un cycle
        /// </summary>
        /// <returns>True ou False</returns>
        public bool ContientUnCycle()
{
    HashSet<T> visites = new HashSet<T>();

    foreach (var noeud in noeuds.Values)
    {
        if (!visites.Contains(noeud.Id))
        {
            if (DFS_DetectCycle(noeud, visites, default(T))) return true;
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
        private bool DFS_DetectCycle(Noeud<T> courant, HashSet<T> visites, T parentId)
{
    visites.Add(courant.Id);

    foreach (var lien in courant.Liens)
    {
        Noeud<T> voisin = lien.Destination;

        if (!visites.Contains(voisin.Id))
        {
            if (DFS_DetectCycle(voisin, visites, courant.Id)) return true;
        }
        else if (!voisin.Id.Equals(parentId))
        {
            return true;
        }
    }

    return false;
}
        #endregion


        #region Dijkstra
        public (Dictionary<T, double> distances, Dictionary<T, T?> precedent) Dijkstra(T source)
{
    var distances = new Dictionary<T, double>();
    var precedent = new Dictionary<T, T?>();
    var priorityQueue = new SortedSet<(double distance, T noeud)>();

    foreach (var noeud in noeuds.Keys)
    {
        distances[noeud] = double.PositiveInfinity;
        precedent[noeud] = default;
    }

    distances[source] = 0;
    priorityQueue.Add((0, source));

    while (priorityQueue.Count > 0)
    {
        var (currentDistance, currentNode) = priorityQueue.Min;
        priorityQueue.Remove(priorityQueue.Min);

        foreach (var lien in noeuds[currentNode].Liens)
        {
            T voisin = lien.Destination.Id;
            double nouvelleDistance = currentDistance + lien.Poids;

            if (nouvelleDistance < distances[voisin])
            {
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
        public (Dictionary<T, double> distances, Dictionary<T, T?> precedent) BellmanFord(T source)
{
    var distances = new Dictionary<T, double>();
    var precedent = new Dictionary<T, T?>();

    foreach (var noeud in noeuds.Keys)
    {
        distances[noeud] = double.PositiveInfinity;
        precedent[noeud] = default;
    }
    distances[source] = 0;

    int nombreSommets = noeuds.Count;

    for (int i = 0; i < nombreSommets - 1; i++)
    {
        foreach (var noeud in noeuds.Values)
        {
            foreach (var lien in noeud.Liens)
            {
                T u = noeud.Id;
                T v = lien.Destination.Id;
                double poids = lien.Poids;

                if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                {
                    distances[v] = distances[u] + poids;
                    precedent[v] = u;
                }
            }
        }
    }

    foreach (var noeud in noeuds.Values)
    {
        foreach (var lien in noeud.Liens)
        {
            T u = noeud.Id;
            T v = lien.Destination.Id;
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
