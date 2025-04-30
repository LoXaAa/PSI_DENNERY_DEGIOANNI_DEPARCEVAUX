using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace PSI_RENDU1
{
    public static class GrapheVisualisation
    {

#region Palette de couleurs
        /// <summary>
        /// Palette de pinceaux pour la coloration algorithmique
        /// </summary>
        private static readonly Brush[] PaletteCouleurs = new Brush[]
        {
            Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Orange,
            Brushes.Purple, Brushes.Brown, Brushes.Cyan, Brushes.Magenta,
            Brushes.Yellow, Brushes.Teal
        };
#endregion

#region Affichage Graphe

        /// <summary>
        /// Génère une image du graphe
        /// </summary>
        /// <param name="graphe">Le graphe à afficher</param>
        /// <param name="cheminSortie">Chemin du fichier de sortie</param>
        /// <param name="largeur">Largeur de l'image en pixels</param>
        /// <param name="hauteur">Hauteur de l'image en pixels</param>
        /// <param name="utiliserIndexCouleur">Si vrai, coloration selon ColorIndex, sinon selon NumLigne</param>

        public static void GenererImageGraphe<T>(Graphe<T> graphe,string cheminSortie = "graphe.png",int largeur = 2000,int hauteur = 1400,bool utiliserIndexCouleur = false)
        {
            Bitmap image = new Bitmap(largeur, hauteur);
            Graphics contexte = Graphics.FromImage(image);
            contexte.Clear(Color.White);

            Pen styloLien = new Pen(Color.Black, 1);
            Brush pinceauTexte = Brushes.Black;
            Font police = new Font("Arial", 8);
            int rayon = 8;

            var latitudes = graphe.Noeuds.Values.Select(n => n.Latitude);
            var longitudes = graphe.Noeuds.Values.Select(n => n.Longitude);

            double minLat = latitudes.Min();
            double maxLat = latitudes.Max();
            double minLong = longitudes.Min();
            double maxLong = longitudes.Max();

            float margeX = 100;
            float margeY = 100;
            double echelleX = (largeur - 2 * margeX) / (maxLong - minLong);
            double echelleY = (hauteur - 2 * margeY) / (maxLat - minLat);
            double decalageX = (largeur - (maxLong - minLong) * echelleX) / 2;
            double decalageY = (hauteur - (maxLat - minLat) * echelleY) / 2;

            var positions = new Dictionary<T, PointF>();
            foreach (var n in graphe.Noeuds.Values)
            {
                float x = (float)((n.Longitude - minLong) * echelleX + decalageX);
                float y = (float)((maxLat - n.Latitude) * echelleY + decalageY);
                positions[n.Id] = new PointF(x, y);
            }

            foreach (var lien in graphe.Liens)
            {
                PointF p1 = positions[lien.Source.Id];
                PointF p2 = positions[lien.Destination.Id];
                contexte.DrawLine(styloLien, p1, p2);
                if (graphe.EstOriente)
                {
                    float dx = p2.X - p1.X;
                    float dy = p2.Y - p1.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist > 0)
                    {
                        float ux = dx / dist;
                        float uy = dy / dist;
                        float flecheX = p2.X - ux * 10;
                        float flecheY = p2.Y - uy * 10;
                        PointF[] pointe = new PointF[]
                        {
                            new PointF(flecheX, flecheY),
                            new PointF(flecheX - uy * 4 - ux * 4, flecheY + ux * 4 - uy * 4),
                            new PointF(flecheX + uy * 4 - ux * 4, flecheY - ux * 4 - uy * 4)
                        };
                        contexte.FillPolygon(Brushes.Black, pointe);
                    }
                }
            }

            foreach (var n in graphe.Noeuds.Values)
            {
                PointF p = positions[n.Id];
                Brush pinceauNoeud = utiliserIndexCouleur
                    ? PaletteCouleurs[n.ColorIndex % PaletteCouleurs.Length]
                    : GetCouleurPourLigne(n.NumLigne);
                contexte.FillEllipse(pinceauNoeud, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                contexte.DrawEllipse(Pens.Black, p.X - rayon, p.Y - rayon, 2 * rayon, 2 * rayon);
                contexte.DrawString(n.Nom, police, pinceauTexte, p.X + rayon, p.Y);
            }

            image.Save(cheminSortie);
            image.Dispose();

            Process.Start(new ProcessStartInfo
            {
                FileName = cheminSortie,
                UseShellExecute = true
            });
        }
#endregion

#region Coloration Ligne
        /// <summary>
        /// Fonction estétique pour la couleur des noeuds selon les numéros de ligne
        /// </summary>
        /// <param name="ligne">Le numéro de la ligne</param>
        /// <returns>Un pinceau correspondant à la ligne</returns>

        private static Brush GetCouleurPourLigne(string ligne)
        {
            switch (ligne)
            {
                case "1":   return Brushes.Yellow;
                case "2":   return Brushes.Blue;
                case "3":   return Brushes.Green;
                case "3bis":return Brushes.LightGreen;
                case "4":   return Brushes.Purple;
                case "5":   return Brushes.Orange;
                case "6":   return Brushes.LightSeaGreen;
                case "7":   return Brushes.Pink;
                case "7bis":return Brushes.LightPink;
                case "8":   return Brushes.Violet;
                case "9":   return Brushes.Gold;
                case "10":  return Brushes.Goldenrod;
                case "11":  return Brushes.Brown;
                case "12":  return Brushes.DarkGreen;
                case "13":  return Brushes.SeaGreen;
                case "14":  return Brushes.Magenta;
                default:    return Brushes.Gray;
            }
        }
#endregion
    }
}
