using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_RENDU1
{

    public class Lien
    {
        public Noeud Source { get; }
        public Noeud Destination { get; }
        public double Poids { get; }

        public Lien(Noeud source, Noeud destination, double poids)
        {
            Source = source;
            Destination = destination;
            Poids = poids;
        }
    }

}
