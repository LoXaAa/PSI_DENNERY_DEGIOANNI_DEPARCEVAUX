using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_RENDU1
{
    public class Noeud
    {
        #region Constructeur + propriété
        public int Id { get; }
        public List<Lien> Liens { get; }

        public Noeud(int id)
        {
            Id = id;
            Liens = new List<Lien>();
        }
        #endregion
    }
}
