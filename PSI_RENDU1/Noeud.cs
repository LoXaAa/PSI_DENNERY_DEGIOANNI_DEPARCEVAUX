using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_RENDU1
{
    public class Noeud<T>
    {
    public T Id { get; }
    public string Nom { get; }
    public double Longitude { get; }
    public double Latitude { get; }
    public List<Lien<T>> Liens { get; }

    public Noeud(T id, string nom = "", double longitude = 0, double latitude = 0)
    {
        Id = id;
        Nom = nom;
        Longitude = longitude;
        Latitude = latitude;
        Liens = new List<Lien<T>>();
    }

    public override string ToString() => Id.ToString();
    }
}
