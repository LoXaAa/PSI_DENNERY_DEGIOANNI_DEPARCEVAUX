using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace PSI_RENDU1
{

   public class GrapheVisualisation
{
    public static void GenererImageGraphe<T>(Graphe<T> graphe, string outputPath = "graphe.png", int width = 2000, int height = 1400)
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

            float marginX = 100;
            float marginY = 100;

            double scaleX = (width - 2 * marginX) / (maxLong - minLong);
            double scaleY = (height - 2 * marginY) / (maxLat - minLat);
            double offsetX = (width - (maxLong - minLong) * scaleX) / 2;
            double offsetY = (height - (maxLat - minLat) * scaleY) / 2;

            Dictionary<T, PointF> positions = new Dictionary<T, PointF>();

            foreach (var noeud in graphe.Noeuds.Values)
            {
                float x = (float)((noeud.Longitude - minLong) * scaleX + offsetX);
                float y = (float)((maxLat - noeud.Latitude) * scaleY + offsetY);
                positions[noeud.Id] = new PointF(x, y);
            }

            foreach (var lien in graphe.Liens)
            {
                PointF p1 = positions[lien.Source.Id];
                PointF p2 = positions[lien.Destination.Id];
                g.DrawLine(lienPen, p1, p2);

                if (graphe.EstOriente)
                {
                    float dx = p2.X - p1.X;
                    float dy = p2.Y - p1.Y;
                    float longueur = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (longueur == 0) continue;

                    float ux = dx / longueur;
                    float uy = dy / longueur;

                    float arrowX = p2.X - ux * 10;
                    float arrowY = p2.Y - uy * 10;

                    PointF[] fleche =
                    {
                        new PointF(arrowX, arrowY),
                        new PointF(arrowX - uy * 4 - ux * 4, arrowY + ux * 4 - uy * 4),
                        new PointF(arrowX + uy * 4 - ux * 4, arrowY - ux * 4 - uy * 4)
                    };
                    g.FillPolygon(Brushes.Black, fleche);
                }
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
