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
        #region Methodes
        public static void GenererImageGraphe(Graphe graphe, string outputPath = "graphe.png", int width = 800, int height = 600)
        {
            try
            {
                Bitmap bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.White);

                Pen lienPen = new Pen(Color.Black, 2);
                Brush noeudBrush = Brushes.LightBlue;
                Brush texteBrush = Brushes.Black;
                Font font = new Font("Arial", 10);
                int rayon = 15;
                Random random = new Random();

                // Générer aléatoire de la position des noeuds
                Dictionary<int, Point> positionsNoeuds = new Dictionary<int, Point>();
                foreach (var noeud in graphe.Noeuds.Values)
                {
                    int x = random.Next(rayon, width - rayon);
                    int y = random.Next(rayon, height - rayon);
                    positionsNoeuds[noeud.Id] = new Point(x, y);
                }


                // Dessin des liens
                foreach (var lien in graphe.Liens)
                {
                    Point p1 = positionsNoeuds[lien.Source.Id];
                    Point p2 = positionsNoeuds[lien.Destination.Id];
                    g.DrawLine(lienPen, p1, p2);
                }

                // Dessiner les nœuds
                foreach (var noeud in graphe.Noeuds.Values)
                {
                    Point p = positionsNoeuds[noeud.Id];
                    g.FillEllipse(noeudBrush, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                    g.DrawEllipse(lienPen, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                    g.DrawString(noeud.Id.ToString(), font, texteBrush, p.X - 5, p.Y - 5);
                }

                bitmap.Save(outputPath);
                bitmap.Dispose();

                Process.Start(new ProcessStartInfo()
                {
                    FileName = outputPath,
                    UseShellExecute = true
                });

                Console.WriteLine($"Image du graphe enregistrée : {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la génération du graphe : {ex.Message}");
            }
        }
        #endregion
    }
}
