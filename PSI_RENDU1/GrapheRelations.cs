using System.Collections.Generic;
using System.Linq;

namespace PSI_RENDU1
{

    public static class ConstructeurGrapheRelations
    {
#region  COnstruction graphe
        /// <summary>
        /// Construit un Graphe chaque client est "C{id}" et chaque cuisinier "U{id}"
        /// </summary>
        /// <param name="relations"> Liste de tuples (idClient, idCuisinier) représentant chaque commande</param>
        /// <returns>Le graphe biparti clients–cuisiniers</returns>
         public static Graphe<string> ConstruireDepuisBDD()
        {
            var relations = DatabaseExtensions.LireRelationsCommande();
            return Construire(relations);
        }
        public static Graphe<string> Construire(IEnumerable<(int idClient, int idCuisinier)> relations)
        {
            var graphe = new Graphe<string>();
            var clients = relations.Select(r => r.idClient).Distinct();
            var cuisiniers = relations.Select(r => r.idCuisinier).Distinct();

            foreach (var idC in clients)
                graphe.AjouterNoeud($"C{idC}");
            foreach (var idU in cuisiniers)
                graphe.AjouterNoeud($"U{idU}");

            foreach (var (idC, idU) in relations)
            {
                string cleint = $"C{idC}";
                string cuisinier = $"U{idU}";
                graphe.AjouterLien(cleint, cuisinier, 1);
                graphe.AjouterLien(cuisinier, cleint, 1);
            }

            return graphe;
        }
#endregion
    }
}
