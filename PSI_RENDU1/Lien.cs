using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_RENDU1
{

    public class Lien<T>
{
    public Noeud<T> Source { get; }
    public Noeud<T> Destination { get; }
    public double Poids { get; }

    public Lien(Noeud<T> source, Noeud<T> destination, double poids)
    {
        Source = source;
        Destination = destination;
        Poids = poids;
    }

    public override string ToString() =>
        $"{Source.Id} -> {Destination.Id} ({Poids})";
}


}
