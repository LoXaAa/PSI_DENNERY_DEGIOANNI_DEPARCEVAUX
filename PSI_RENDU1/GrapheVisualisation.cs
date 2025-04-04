using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace PSI_RENDU1
{

    public class GrapheVisualisation
{
    public static void GenererImageGraphe<T>(Graphe<T> graphe, string outputPath = "graphe.png", int width = 2500, int height = 1500)
    {
        try
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            Pen lienPen = new Pen(Color.Black, 1);
            Brush noeudBrush = Brushes.LightBlue;
            Brush texteBrush = Brushes.Black;
            Font font = new Font("Arial", 8);
            int rayon = 8;

            var latitudes = graphe.Noeuds.Values.Select(n => n.Latitude).ToList();
            var longitudes = graphe.Noeuds.Values.Select(n => n.Longitude).ToList();

            double minLat = latitudes.Min();
            double maxLat = latitudes.Max();
            double minLong = longitudes.Min();
            double maxLong = longitudes.Max();

            // 🧭 Marges autour du graphe
            float marginX = 100;
            float marginY = 100;

            double scaleX = (width - 2 * marginX) / (maxLong - minLong);
            double scaleY = (height - 2 * marginY) / (maxLat - minLat);

            Dictionary<T, PointF> positions = new Dictionary<T, PointF>();

            foreach (var noeud in graphe.Noeuds.Values)
            {
                float x = marginX + (float)((noeud.Longitude - minLong) * scaleX);
                float y = marginY + (float)((maxLat - noeud.Latitude) * scaleY);
                positions[noeud.Id] = new PointF(x, y);
            }

            foreach (var lien in graphe.Liens)
            {
                PointF p1 = positions[lien.Source.Id];
                PointF p2 = positions[lien.Destination.Id];
                g.DrawLine(lienPen, p1, p2);
            }

            foreach (var noeud in graphe.Noeuds.Values)
            {
                PointF p = positions[noeud.Id];
                g.FillEllipse(noeudBrush, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                g.DrawEllipse(Pens.Black, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                g.DrawString(noeud.Nom, font, texteBrush, p.X + rayon, p.Y);
            }

            bitmap.Save(outputPath);
            bitmap.Dispose();

            Process.Start(new ProcessStartInfo()
            {
                FileName = outputPath,
                UseShellExecute = true
            });

            Console.WriteLine($"✅ Image du graphe enregistrée : {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la génération du graphe : {ex.Message}");
        }
    }
}
}
