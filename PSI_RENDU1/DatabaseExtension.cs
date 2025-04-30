using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace PSI_RENDU1
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Lit tous les couples (Id_Client, Id_Cuisinier) de la table Commande.
        /// </summary>
        public static IEnumerable<(int idClient, int idCuisinier)> LireRelationsCommande()
{
    var liste = new List<(int, int)>();
    try
    {
        Database.OpenConnection();
        using var cmd = new MySqlCommand(
            "SELECT Id_Client, Id_Cuisinier FROM Commande " +
            "WHERE Id_Client IS NOT NULL AND Id_Cuisinier IS NOT NULL",
            Database.GetConnection());
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            liste.Add((
                rdr.GetInt32("Id_Client"),
                rdr.GetInt32("Id_Cuisinier")
            ));
        }
    }
    finally
    {
        Database.CloseConnection();
    }
    return liste;
}
    }
}
