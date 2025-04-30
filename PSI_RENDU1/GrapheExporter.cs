using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace PSI_RENDU1
{
    public static class GraphExporter<T>
    {

#region Export Json

        /// <summary>
        /// Exporte la liste des nœuds au format JSON
        /// </summary>
        /// <param name="chemin">Chemin du fichier de sortie</param>
        /// <param name="graphe">Le graphe dont on exporte les nœuds</param>

        public static void ExporterEnJson(string chemin, Graphe<T> graphe)
        {
            var noeudsAExporter = graphe.Noeuds.Values
                .Select(n => new { n.Id, n.Nom, n.ColorIndex })
                .ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string contenuJson = JsonSerializer.Serialize(noeudsAExporter, options);
            File.WriteAllText(chemin, contenuJson);
        }
#endregion

#region Export XML

        /// <summary>
        /// Exporte la liste des nœuds avec leur ColorIndex au format XML
        /// </summary>
        /// <param name="chemin">Chemin du fichier de sortie</param>
        /// <param name="graphe">Le graphe dont on exporte les nœuds</param>

        public static void ExporterEnXml(string chemin, Graphe<T> graphe)
        {
            var racine = new XElement("Noeuds",
                graphe.Noeuds.Values.Select(n =>
                    new XElement("Noeud",
                        new XAttribute("Id", n.Id),
                        new XElement("Nom", n.Nom),
                        new XElement("IndexCouleur", n.ColorIndex)
                    )
                )
            );
            var document = new XDocument(racine);
            document.Save(chemin);
        }

#endregion
    }
}
