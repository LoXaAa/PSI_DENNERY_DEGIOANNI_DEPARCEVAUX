using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace PSI_RENDU1{
public static class LecteurExcel
{
    public static List<(int id, string nom, double longitude, double latitude)> LireNoeuds(string cheminFichier)
{
    var resultats = new List<(int, string, double, double)>();

    using (var workbook = new XLWorkbook(cheminFichier))
    {
        var feuille = workbook.Worksheet("Noeuds");

        foreach (var ligne in feuille.RowsUsed().Skip(1))
        {
            try
            {
                if (!int.TryParse(ligne.Cell(1).GetString().Trim(), out int id)) continue;
                string nom = ligne.Cell(3).GetString().Trim();

                string longStr = ligne.Cell(4).GetString().Trim();
                string latStr = ligne.Cell(5).GetString().Trim();

                if (!double.TryParse(longStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude)) continue;
                if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude)) continue;

                resultats.Add((id, nom, longitude, latitude));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur de lecture noeud : {ex.Message}");
            }
        }
    }

    return resultats;
}

    public static List<(int, int, double, bool)> LireArcs(string cheminFichier)
{
    var arcs = new List<(int, int, double, bool)>();
    using var classeur = new XLWorkbook(cheminFichier);
    var feuille = classeur.Worksheet("Arcs");

    foreach (var ligne in feuille.RowsUsed().Skip(1))
    {
        try
        {
            string sourceStr = ligne.Cell(1).GetString().Trim();
            string destStr = ligne.Cell(4).GetString().Trim();
            string poidsStr = ligne.Cell(5).GetString().Trim();
            string sensStr = ligne.Cell(7).GetString().Trim().ToLower();

            if (string.IsNullOrEmpty(sourceStr) || string.IsNullOrEmpty(destStr) || string.IsNullOrEmpty(poidsStr))
                continue;

            int source = int.Parse(sourceStr);
            int destination = int.Parse(destStr);
            double poids = double.Parse(poidsStr, CultureInfo.InvariantCulture);
            bool sensUnique = sensStr == "oui";

            arcs.Add((source, destination, poids, sensUnique));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erreur de lecture arc : {ex.Message}");
        }
    }

    return arcs;
}


    public static List<(int, int)> TrouverCorrespondances(List<(int id, string nom, double, double)> noeuds)
{
    var groupes = noeuds
        .GroupBy(n => n.nom.Trim().ToLower())
        .Where(g => g.Count() > 1)
        .ToList();

    var correspondances = new List<(int, int)>();

    foreach (var groupe in groupes)
    {
        var ids = groupe.Select(n => n.id).ToList();
        for (int i = 0; i < ids.Count; i++)
        {
            for (int j = i + 1; j < ids.Count; j++)
            {
                correspondances.Add((ids[i], ids[j]));
            }
        }
    }

    return correspondances;
}
}
}
