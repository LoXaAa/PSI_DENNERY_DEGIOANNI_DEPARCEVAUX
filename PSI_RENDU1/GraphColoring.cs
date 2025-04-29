using System.Linq;

namespace PSI_RENDU1
{

    public static class GraphColoring<T>
    {

#region WelshPowell

        /// <summary>
        /// Algorithme de WelshPowell
        /// </summary>
        /// <param name="graphe">Le graphe auquel appliquer l'algorithme</param>

        public static int WelshPowell(Graphe<T> graphe)
        {
            var listeNoeudsTries = graphe.Noeuds.Values
                .OrderByDescending(noeud => noeud.Liens.Count)
                .ToList();

            int couleurCourante = 0;

            foreach (var noeud in listeNoeudsTries)
            {
                if (noeud.ColorIndex != -1)
                    continue;
                noeud.ColorIndex = couleurCourante;

                foreach (var autreNoeud in listeNoeudsTries)
                {
                    if (autreNoeud.ColorIndex != -1)
                        continue;

                    bool conflit = graphe.Liens
                        .Where(lien => lien.Source == autreNoeud || lien.Destination == autreNoeud)
                        .Select(lien =>
                            lien.Source == autreNoeud
                                ? lien.Destination
                                : lien.Source)
                        .Any(voisin => voisin.ColorIndex == couleurCourante);

                    if (!conflit)
                        autreNoeud.ColorIndex = couleurCourante;
                }

                couleurCourante++;
            }

            return couleurCourante;
        }
#endregion
    }
}
