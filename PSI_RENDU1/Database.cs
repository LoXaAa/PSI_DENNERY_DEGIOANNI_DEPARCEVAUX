using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_RENDU1
{
    public class Database
    {
        private MySqlConnection connection;
        #region fonctions de connection à la database
        public Database()
        {
            Console.WriteLine("Test du projet");
            try
            {
                string connectionString = "Server=localhost;Database=livparis;User ID=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=True;";
                connection = new MySqlConnection(connectionString);
                Console.WriteLine("Connexion réussie !");
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur de connexion : " + e.Message);
            }
        }
        public void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }
        public MySqlConnection GetConnection()
        {
            return connection;
        }
        #endregion
        #region fonction de peuplement
        static public int AjouterCompte()
        {
            Program.Titre();
            Database db = new Database();

            Console.WriteLine("Création de compte\n");

            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            Console.Write("Rue : ");
            string rue = Console.ReadLine();

            Console.Write("Numéro de rue : ");
            int numero;
            while (!int.TryParse(Console.ReadLine(), out numero))
            {
                Console.Write("Veuillez entrer un numéro valide : ");
            }

            Console.Write("Code postal : ");
            int codePostal;
            while (!int.TryParse(Console.ReadLine(), out codePostal))
            {
                Console.Write("Veuillez entrer un code postal valide : ");
            }

            Console.Write("Ville : ");
            string ville = Console.ReadLine();

            Console.Write("Numéro de téléphone : ");
            string telephone = Console.ReadLine();

            Console.Write("Email : ");
            string email = Console.ReadLine();

            Console.Write("Station de métro la plus proche : ");
            string stationMetro = Console.ReadLine();

            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine(); // Il serait préférable de le hasher avant de le stocker.

            int idCompte = -1;

            try
            {
                db.OpenConnection();
                string query = "INSERT INTO Compte (Prenom, Nom, Rue, Numero, Code_postal, Ville, No_tel, Email, Station_de_Métro_la_plus_Proche, Statut, Mot_Passe) " +
                               "VALUES (@Prenom, @Nom, @Rue, @Numero, @CodePostal, @Ville, @Telephone, @Email, @StationMetro, 'ouvert', @MotPasse)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@Prenom", prenom);
                cmd.Parameters.AddWithValue("@Nom", nom);
                cmd.Parameters.AddWithValue("@Rue", rue);
                cmd.Parameters.AddWithValue("@Numero", numero);
                cmd.Parameters.AddWithValue("@CodePostal", codePostal);
                cmd.Parameters.AddWithValue("@Ville", ville);
                cmd.Parameters.AddWithValue("@Telephone", telephone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@StationMetro", stationMetro);
                cmd.Parameters.AddWithValue("@MotPasse", motDePasse);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Compte créé avec succès !");
                    MySqlCommand getIdCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", db.GetConnection());
                    idCompte = Convert.ToInt32(getIdCmd.ExecuteScalar());
                }
                else
                {
                    Console.WriteLine("Échec de la création du compte.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }

            return idCompte;
        }
        static public void AjouterClient(int idCompte)
        {
            Program.Titre();
            if (idCompte <= 0)
            {
                Console.WriteLine("ID de compte invalide.");
                return;
            }

            Database db = new Database();

            Console.Write("Nom de l'entreprise : ");
            string nomEntreprise = Console.ReadLine();

            try
            {
                db.OpenConnection();
                string query = "INSERT INTO Client (Nom_Entreprise, Id_Compte) VALUES (@NomEntreprise, @IdCompte)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@NomEntreprise", nomEntreprise);
                cmd.Parameters.AddWithValue("@IdCompte", idCompte);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Client créé avec succès !");
                }
                else
                {
                    Console.WriteLine("Échec de la création du client.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void AjouterCuisinier(int idCompte)
        {
            Program.Titre();
            if (idCompte <= 0)
            {
                Console.WriteLine("ID de compte invalide.");
                return;
            }

            Database db = new Database();

            Console.Write("Zone de livraison : ");
            string zoneLivraison = Console.ReadLine();

            try
            {
                db.OpenConnection();
                string query = "INSERT INTO Cuisinier (Zone_Livraison, Id_Compte) VALUES (@ZoneLivraison, @IdCompte)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@ZoneLivraison", zoneLivraison);
                cmd.Parameters.AddWithValue("@IdCompte", idCompte);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Cuisinier créé avec succès !");
                }
                else
                {
                    Console.WriteLine("Échec de la création du cuisinier.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void AjouterRecette()
        {
            Database db = new Database();

            Console.WriteLine("Création d'une nouvelle recette\n");

            Console.Write("Nom de la recette : ");
            string nomRecette = Console.ReadLine();

            Console.Write("Instructions : ");
            string instructions = Console.ReadLine();

            Console.Write("Temps de préparation (en minutes) : ");
            int tempsPreparation;
            while (!int.TryParse(Console.ReadLine(), out tempsPreparation) || tempsPreparation < 0)
            {
                Console.Write("Veuillez entrer un nombre valide : ");
            }

            Console.Write("Temps de cuisson (en minutes) : ");
            int tempsCuisson;
            while (!int.TryParse(Console.ReadLine(), out tempsCuisson) || tempsCuisson < 0)
            {
                Console.Write("Veuillez entrer un nombre valide : ");
            }

            Console.Write("Difficulté (1-Facile/2-Moyen/3-Difficile) : ");
            int diff;
            while (!int.TryParse(Console.ReadLine(), out diff) || diff < 1 || diff > 3)
            {
                Console.Write("Veuillez entrer une note valide (1-3) : ");
            }
            string difficulte;
            switch (diff)
            {
                case 1:
                    difficulte = "Facile";
                    break;
                case 2:
                    difficulte = "Moyen";
                    break;
                case 3:
                    difficulte = "Difficile";
                    break;
                default:
                    difficulte = "Inconnu"; 
                    break;
            }

            Console.WriteLine("Vous avez choisi la difficulté : "+difficulte);


            try
            {
                db.OpenConnection();
                string query = @"
            INSERT INTO Recette (Nom_Recette, Instructions, Temps_Préparation, Temps_Cuisson, Difficulte) 
            VALUES (@NomRecette, @Instructions, @TempsPreparation, @TempsCuisson, @Difficulte)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@NomRecette", nomRecette);
                cmd.Parameters.AddWithValue("@Instructions", instructions);
                cmd.Parameters.AddWithValue("@TempsPreparation", tempsPreparation);
                cmd.Parameters.AddWithValue("@TempsCuisson", tempsCuisson);
                cmd.Parameters.AddWithValue("@Difficulte", difficulte);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("Recette ajoutée avec succès !");
                else
                    Console.WriteLine("Échec de l'ajout de la recette.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void AjouterIngredient()
        {
            Database db = new Database();

            Console.WriteLine("Ajout d'un nouvel ingrédient\n");

            Console.Write("Nom de l'ingrédient : ");
            string nomIngredient = Console.ReadLine();

            Console.Write("Volume (en grammes/ml) : ");
            int volume;
            while (!int.TryParse(Console.ReadLine(), out volume) || volume < 0)
            {
                Console.Write("Veuillez entrer un volume valide : ");
            }

            try
            {
                db.OpenConnection();
                string query = "INSERT INTO Ingredient (Volume, Ingredient) VALUES (@Volume, @Ingredient)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@Volume", volume);
                cmd.Parameters.AddWithValue("@Ingredient", nomIngredient);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowsAffected > 0 ? "Ingrédient ajouté avec succès !" : "Échec de l'ajout de l'ingrédient.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void AjouterPlat(int idCuisinier)
{
    Database db = new Database();

    Console.WriteLine("Ajout d'un nouveau plat\n");

    // Validation stricte des entrées utilisateur
    Console.Write("Type de plat : ");
    string typePlat;
    while (string.IsNullOrWhiteSpace(typePlat = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer un type de plat valide : ");
    }

    Console.Write("Date de fabrication (YYYY-MM-DD) : ");
    string dateFabrication;
    while (!DateTime.TryParse(Console.ReadLine(), out _))
    {
        Console.Write("Veuillez entrer une date de fabrication valide (YYYY-MM-DD) : ");
    }
    dateFabrication = Console.ReadLine();

    Console.Write("Date de péremption (YYYY-MM-DD) : ");
    string datePeremption;
    while (!DateTime.TryParse(Console.ReadLine(), out _))
    {
        Console.Write("Veuillez entrer une date de péremption valide (YYYY-MM-DD) : ");
    }
    datePeremption = Console.ReadLine();

    Console.Write("Type de régime (ex: Végétarien, Sans gluten) : ");
    string typeRegime;
    while (string.IsNullOrWhiteSpace(typeRegime = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer un type de régime valide : ");
    }

    Console.Write("Photo (URL ou nom du fichier) : ");
    string photo;
    while (string.IsNullOrWhiteSpace(photo = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer un nom de fichier ou une URL valide pour la photo : ");
    }

    Console.Write("Description : ");
    string description;
    while (string.IsNullOrWhiteSpace(description = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer une description valide : ");
    }

    Console.Write("Nationalité : ");
    string nationalite;
    while (string.IsNullOrWhiteSpace(nationalite = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer une nationalité valide : ");
    }

    Console.Write("Prix (€) : ");
    decimal prix;
    while (!decimal.TryParse(Console.ReadLine(), out prix) || prix < 0)
    {
        Console.Write("Veuillez entrer un prix valide (€) : ");
    }

    Console.Write("Nombre de portions : ");
    int nombrePortion;
    while (!int.TryParse(Console.ReadLine(), out nombrePortion) || nombrePortion <= 0)
    {
        Console.Write("Veuillez entrer un nombre de portions valide : ");
    }

    Console.Write("Ingrédients principaux : ");
    string ingredientsPrincipaux;
    while (string.IsNullOrWhiteSpace(ingredientsPrincipaux = Console.ReadLine()))
    {
        Console.Write("Veuillez entrer des ingrédients principaux valides : ");
    }

    Console.Write("ID de la recette (si disponible, sinon 0) : ");
    int idRecette;
    while (!int.TryParse(Console.ReadLine(), out idRecette))
    {
        Console.Write("Veuillez entrer un ID de recette valide (ou 0 si aucune recette) : ");
    }

    try
    {
        db.OpenConnection();
        string query = @"INSERT INTO Plat 
    (Type_Plat, Date_Fabrication, Date_Peremption, Type_Regime, Photo, Description, Nationalité, Prix, Nombre_Portion, Ingrédients_Principaux, Id_Recette, Id_Cuisinier) 
    VALUES 
    (@TypePlat, @DateFabrication, @DatePeremption, @TypeRegime, @Photo, @Description, @Nationalite, @Prix, @NombrePortion, @IngredientsPrincipaux, @IdRecette, @IdCuisinier)";

        MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
        cmd.Parameters.AddWithValue("@TypePlat", typePlat);
        cmd.Parameters.AddWithValue("@DateFabrication", dateFabrication);
        cmd.Parameters.AddWithValue("@DatePeremption", datePeremption);
        cmd.Parameters.AddWithValue("@TypeRegime", typeRegime);
        cmd.Parameters.AddWithValue("@Photo", photo);
        cmd.Parameters.AddWithValue("@Description", description);
        cmd.Parameters.AddWithValue("@Nationalite", nationalite);
        cmd.Parameters.AddWithValue("@Prix", prix);
        cmd.Parameters.AddWithValue("@NombrePortion", nombrePortion);
        cmd.Parameters.AddWithValue("@IngredientsPrincipaux", ingredientsPrincipaux);
        cmd.Parameters.AddWithValue("@IdRecette", idRecette == 0 ? (object)DBNull.Value : idRecette);
        cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);

        int rowsAffected = cmd.ExecuteNonQuery();
        Console.WriteLine(rowsAffected > 0 ? "Plat ajouté avec succès !" : "Échec de l'ajout du plat.");
    }
    catch (MySqlException sqlEx)
    {
        Console.WriteLine("Erreur SQL : " + sqlEx.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur inattendue : " + ex.Message);
    }
    finally
    {
        db.CloseConnection();
    }
}// problème d'implémentation

        static public int AjouterCommande(int idClient, int idCuisinier)
        {
            Database db = new Database();

            Console.WriteLine("Création d'une nouvelle commande\n");

            Console.Write("Statut de la commande (En attente, En cours, Livrée) : ");
            string statutCommande = Console.ReadLine();

            Console.Write("Statut de la transaction (En attente, Payé, Annulé) : ");
            string statutTransaction = Console.ReadLine();

            Console.Write("Date de paiement (YYYY-MM-DD, laisser vide si non payée) : ");
            string datePaiement = Console.ReadLine();
            object datePaiementObj = string.IsNullOrEmpty(datePaiement) ? DBNull.Value : datePaiement;

            Console.Write("Mode de paiement (CB, PayPal, Espèces, etc.) : ");
            string modePaiement = Console.ReadLine();

            int idCommande = -1;

            try
            {
                db.OpenConnection();

                string query = @"INSERT INTO Commande 
                (Date_Commande, Statut_Commande, Prix_Total, Statut_Transaction, Date_Paiement, Mode_Paiement, Id_Client, Id_Cuisinier) 
                VALUES 
                (@DateCommande, @StatutCommande, 0, @StatutTransaction, @DatePaiement, @ModePaiement, @IdClient, @IdCuisinier)";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@DateCommande", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@StatutCommande", statutCommande);
                cmd.Parameters.AddWithValue("@StatutTransaction", statutTransaction);
                cmd.Parameters.AddWithValue("@DatePaiement", datePaiementObj);
                cmd.Parameters.AddWithValue("@ModePaiement", modePaiement);
                cmd.Parameters.AddWithValue("@IdClient", idClient);
                cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Récupérer l'ID de la commande insérée
                    cmd.CommandText = "SELECT LAST_INSERT_ID()";
                    idCommande = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine("Commande créée avec succès. ID de la commande : " + idCommande);
                }
                else
                {
                    Console.WriteLine("Échec de la création de la commande.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }

            return idCommande;
        }

        static public void AjouterAvis(int idClient, int idCuisinier)
        {
            Database db = new Database();
            Console.WriteLine("Création d'un avis\n");
            Console.Write("Note (1-5) : ");
            int note;
            while (!int.TryParse(Console.ReadLine(), out note) || note < 1 || note > 5)
            {
                Console.Write("Veuillez entrer une note valide (1-5) : ");
            }
            Console.Write("Commentaire : ");
            string commentaire = Console.ReadLine();

            DateTime dateAvis = DateTime.Now;

            try
            {
                db.OpenConnection();
                string query = @"INSERT INTO Avis 
                            (Note, Date_Avis, Commentaire, Id_Client, Id_Cuisinier) 
                            VALUES 
                            (@Note, @DateAvis, @Commentaire, @IdClient, @IdCuisinier)";

                using (MySqlCommand cmd = new MySqlCommand(query, db.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Note", note);
                    cmd.Parameters.AddWithValue("@DateAvis", dateAvis);
                    cmd.Parameters.AddWithValue("@Commentaire", commentaire);
                    cmd.Parameters.AddWithValue("@IdClient", idClient);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Avis ajouté avec succès !");
                    else
                        Console.WriteLine("Erreur lors de l'ajout de l'avis.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        #endregion
        #region fonctions de connexion au compte
        static public void ConnexionClient(int idCompte)
        {
            Console.WriteLine("Avez vous déjà un compte client ? (O/N) : ");
            string reponse = RepOuiNon();
            if (reponse == "n")
            {
                Console.WriteLine("Vous devez d'abord créer un compte client.");
                AjouterClient(idCompte);
                return;
            }
            if (idCompte <= 0)
            {
                Console.WriteLine("ID de compte invalide.");
                return;
            }

            Database db = new Database();

            try
            {
                db.OpenConnection();

                // Vérifier si un compte client existe avec cet Id_Compte
                string query = @"
            SELECT Client.Id_Client 
            FROM Client 
            INNER JOIN Compte ON Client.Id_Compte = Compte.Id_Compte
            WHERE Client.Id_Compte = @IdCompte";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdCompte", idCompte);

                object result = cmd.ExecuteScalar();

                if (result == null) // Aucun compte client trouvé
                {
                    Console.WriteLine("Aucun compte client trouvé avec cet identifiant.");
                    Console.WriteLine("Voulez-vous en créer un ? (O/N) :");

                    string reponse1 = RepOuiNon();
                    if (reponse1 == "o")
                    {
                        AjouterClient(idCompte);
                    }
                    return;
                }

                Console.WriteLine("Connexion réussie au compte client !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la connexion au compte client : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }


        }
        static public void ConnexionCuisinier(int idCompte)
        {
            Console.WriteLine("Avez vous déjà un compte cuisinier ? (O/N) :");
            string reponse = RepOuiNon();
            if (reponse == "n")
            {
                Console.WriteLine("Vous devez d'abord créer un compte cuisinier.");
                AjouterCuisinier(idCompte);
                return;
            }


            if (idCompte <= 0)
            {
                Console.WriteLine("ID de compte invalide.");
                return;
            }

            Database db = new Database();

            try
            {
                db.OpenConnection();

                // Vérifier si un compte cuisinier existe avec ce Id_Compte
                string query = @"
            SELECT Cuisinier.Id_Cuisinier 
            FROM Cuisinier 
            INNER JOIN Compte ON Cuisinier.Id_Compte = Compte.Id_Compte
            WHERE Cuisinier.Id_Compte = @IdCompte";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdCompte", idCompte);

                object result = cmd.ExecuteScalar();

                if (result == null) // Aucun compte cuisinier trouvé
                {
                    Console.WriteLine("Aucun compte cuisinier trouvé avec cet identifiant.");
                    Console.WriteLine("Voulez-vous en créer un ? (O/N) :");

                    string reponse1 = RepOuiNon();
                    if (reponse1 == "o")
                    {
                        AjouterCuisinier(idCompte);
                    }
                    return;
                }

                Console.WriteLine("Connexion réussie au compte cuisinier !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la connexion au compte cuisinier : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }

        }

        static public bool ConnexionCompte(int idCompte)
        {
            Program.Titre();
            Database db = new Database();
            Console.WriteLine("Connexion à votre compte\n");
            Console.Write("\nMot de passe: ");
            string motDePasse = Console.ReadLine();
            try
            {
                db.OpenConnection();
                string query = "SELECT Id_Compte FROM Compte WHERE Id_Compte = @IdCompte AND Mot_Passe = @MotDePasse";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdCompte", idCompte);
                cmd.Parameters.AddWithValue("@MotDePasse", motDePasse);
                object result = cmd.ExecuteScalar();
                bool rep;
                if (result != null)
                {
                    Console.WriteLine("Connexion réussie !");
                    Console.WriteLine("ID du compte : " + result);
                    return true;
                }
                else
                {
                    Console.WriteLine("ID du compte ou mot de passe incorrect.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
            return false;
        }
        #endregion
        #region fonction de supression
        static public void Supprimer(string element)
        {
            Database db = new Database();

            Console.Write("Entrez l'ID du(/de l') " + element + " à supprimer : ");
            int idElement;
            while (!int.TryParse(Console.ReadLine(), out idElement))
            {
                Console.Write("Veuillez entrer un ID valide : ");
            }

            Console.Write("Êtes-vous sûr de vouloir supprimer ce/cet " + element + " ? (O/N) : ");
            string confirmation = RepOuiNon();
            if (confirmation.ToLower() != "o")
            {
                Console.WriteLine("Suppression annulée.");
                return;
            }

            try
            {
                db.OpenConnection();
                string query = "DELETE FROM " + element + " WHERE Id_" + element + " = @Id" + element;
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@Id" + element, idElement);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine(element + " supprimé avec succès !");
                else
                    Console.WriteLine("Aucun " + element + " trouvé avec cet ID.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        #endregion
        #region fonction de saisie
        static string RepOuiNon()
        {
            string reponse;
            while ((reponse = Console.ReadLine().ToLower()) != "o" && reponse != "n")
            {
                Console.Write("Veuillez répondre par O ou N : ");
            }
            return reponse;
        }
        #endregion
        #region Fonctions montrer
        static public void MontrerEssentiel(string element)
        {
            Database db = new Database();
            try
            {
                db.OpenConnection();
                string query = $@"
                SELECT e.Id_{element}, c.Prenom, c.Nom
                FROM {element} e
                INNER JOIN Compte c ON e.Id_Compte = c.Id_Compte;";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                MySqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("\nListe de "+element+" :\n");

                while (reader.Read())
                {
                    Console.WriteLine($"\tID : {reader[$"Id_{element}"]}\t Nom : {reader["Nom"]}\t Prénom : {reader["Prenom"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }// valable que pour cuisinier, client et compte
        static public void Montrer(string element)
        {
            Database db = new Database();
            try
            {
                db.OpenConnection();
                string query = "SELECT * FROM " + element;
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Résultats trouvés pour l'élément " + element + " :\n");
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetName(i) + ": " + reader[i] + "\t");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Aucun résultat trouvé dans " + element + ".");
                    }
                }
            }
            catch (MySqlException mySqlEx)
            {
                Console.WriteLine("Erreur SQL : " + mySqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur inattendue : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void Montrer(string element, int id)
        {
            Database db = new Database();
            try
            {
                db.OpenConnection();
                string query = "SELECT * FROM " + element + " WHERE Id_" + element + " = @Id";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@Id", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Résultats trouvés pour l'élément " + element + " avec ID " + id + " :\n");
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetName(i) + ": " + reader[i] + "\t");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Aucun résultat trouvé dans " + element + " pour l'ID spécifié : " + id);
                    }
                }
            }
            catch (MySqlException mySqlEx)
            {
                Console.WriteLine("Erreur SQL : " + mySqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur inattendue : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void MontrerProfilCuisinier(int idCuisinier)
        {
            Database db = new Database();
            try
            {
                db.OpenConnection();

                // Requête avec JOIN pour récupérer uniquement les infos du compte associé
                string query = @"
            SELECT 
                Compte.Prenom,
                Compte.Nom,
                Compte.Rue,
                Compte.Numero,
                Compte.Code_postal,
                Compte.Ville,
                Compte.No_tel,
                Compte.Email,
                Compte.Station_de_Métro_la_plus_Proche,
                Cuisinier.Zone_Livraison
            FROM 
                Compte
            INNER JOIN 
                Cuisinier ON Compte.Id_Compte = Cuisinier.Id_Compte
            WHERE 
                Cuisinier.Id_Cuisinier = @IdCuisinier";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Informations du profil cuisinier :\n");
                    while (reader.Read())
                    {
                        Console.WriteLine("Prénom : " + reader["Prenom"]);
                        Console.WriteLine("Nom : " + reader["Nom"]);
                        Console.WriteLine("Adresse : " + reader["Rue"] + " " + reader["Numero"] + ", " + reader["Code_postal"] + " " + reader["Ville"]);
                        Console.WriteLine("Téléphone : " + reader["No_tel"]);
                        Console.WriteLine("Email : " + reader["Email"]);
                        Console.WriteLine("Station de métro la plus proche : " + reader["Station_de_Métro_la_plus_Proche"]);
                        Console.WriteLine("Zone de livraison : " + reader["Zone_Livraison"]);
                    }
                }
                else
                {
                    Console.WriteLine("Aucun profil trouvé pour ce cuisinier.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public void MontrerProfilClient(int idClient)
        {
            Database db = new Database();
            try
            {
                db.OpenConnection();

                // Requête avec JOIN pour récupérer uniquement les infos du compte associé au client
                string query = @"
            SELECT 
                Compte.Prenom,
                Compte.Nom,
                Compte.Rue,
                Compte.Numero,
                Compte.Code_postal,
                Compte.Ville,
                Compte.No_tel,
                Compte.Email,
                Compte.Station_de_Métro_la_plus_Proche,
                Client.Nom_Entreprise
            FROM 
                Compte
            INNER JOIN 
                Client ON Compte.Id_Compte = Client.Id_Compte
            WHERE 
                Client.Id_Client = @IdClient";

                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdClient", idClient);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Informations du profil client :\n");
                    while (reader.Read())
                    {
                        Console.WriteLine("Prénom : " + reader["Prenom"]);
                        Console.WriteLine("Nom : " + reader["Nom"]);
                        Console.WriteLine("Adresse : " + reader["Rue"] + " " + reader["Numero"] + ", " + reader["Code_postal"] + " " + reader["Ville"]);
                        Console.WriteLine("Téléphone : " + reader["No_tel"]);
                        Console.WriteLine("Email : " + reader["Email"]);
                        Console.WriteLine("Station de métro la plus proche : " + reader["Station_de_Métro_la_plus_Proche"]);
                        Console.WriteLine("Nom de l'entreprise : " + reader["Nom_Entreprise"]);
                    }
                }
                else
                {
                    Console.WriteLine("Aucun profil trouvé pour ce client.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        #endregion

       #region fonction de modification
public static void ModifierClient(int idCompte)
{
    try
    {
        OpenConnection(); // Ouvrir la connexion

        Console.WriteLine("Entrez le nouveau nom :");
        string nom = Console.ReadLine();
        Console.WriteLine("Entrez le nouveau prénom :");
        string prenom = Console.ReadLine();
        Console.WriteLine("Entrez le nouvel email :");
        string email = Console.ReadLine();

        string query = "UPDATE clients SET nom = @nom, prenom = @prenom, email = @email WHERE id = @idCompte;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@idCompte", idCompte);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@prenom", prenom);
            command.Parameters.AddWithValue("@email", email);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine("Erreur lors de la modification : "+ex.Message);
    }
    finally
    {
        CloseConnection();
    }
}
public static void ModifierCuisinier(int idCuisinier)
{
    try
    {
        OpenConnection(); // Ouvrir la connexion

        Console.WriteLine("Entrez le nouveau nom :");
        string nom = Console.ReadLine();
        Console.WriteLine("Entrez la nouvelle spécialité :");
        string specialite = Console.ReadLine();

        string query = "UPDATE cuisiniers SET nom = @nom, specialite = @specialite WHERE id = @idCuisinier;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@specialite", specialite);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine("Erreur lors de la modification :" +ex.Message);
    }
    finally
    {
        CloseConnection();
    }
}
public static void ModifierCommande(int idCompte)
{
    MySqlConnection connection = null;

    try
    {
        // Configurer et ouvrir la connexion
        string connectionString = "Server=localhost;Database=livparis;User ID=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=True;";
        connection = new MySqlConnection(connectionString);
        connection.Open();

        Console.WriteLine("Entrez l'ID de la commande à modifier :");
        int idCommande = int.Parse(Console.ReadLine());

        Console.WriteLine("Entrez le nouveau statut de la commande :");
        string statut = Console.ReadLine();
        Console.WriteLine("Entrez la nouvelle date de livraison (yyyy-MM-dd) :");
        DateTime dateLivraison = DateTime.Parse(Console.ReadLine());

        string query = "UPDATE commandes SET statut = @statut, date_livraison = @dateLivraison WHERE id = @idCommande AND id_compte = @idCompte;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            // Ajouter les paramètres
            command.Parameters.AddWithValue("@idCompte", idCompte);
            command.Parameters.AddWithValue("@idCommande", idCommande);
            command.Parameters.AddWithValue("@statut", statut);
            command.Parameters.AddWithValue("@dateLivraison", dateLivraison);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine("Erreur lors de la modification : "+ex.Message);
    }
    catch (FormatException)
    {
        Console.WriteLine("Erreur : Format invalide. Veuillez entrer des valeurs correctes.");
    }
    finally
    {
        // Fermer la connexion
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}

public static void ModifierCompte(int idCompte)
{
    try
    {
        OpenConnection(); // Ouvrir la connexion

        Console.WriteLine("Entrez le nouveau nom d'utilisateur :");
        string nomUtilisateur = Console.ReadLine();
        Console.WriteLine("Entrez le nouveau mot de passe :");
        string motDePasse = Console.ReadLine();

        string query = "UPDATE comptes SET nom_utilisateur = @nomUtilisateur, mot_de_passe = @motDePasse WHERE id = @idCompte;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@idCompte", idCompte);
            command.Parameters.AddWithValue("@nomUtilisateur", nomUtilisateur);
            command.Parameters.AddWithValue("@motDePasse", motDePasse);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected+" ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine("Erreur lors de la modification : " +ex.Message);
    }
    finally
    {
        CloseConnection();
    }
}
public static void ModifierIngredient()
{
    try
    {
        OpenConnection(); // Ouvrir la connexion

        Console.WriteLine("Entrez l'ID de l'ingrédient à modifier :");
        int idIngredient = int.Parse(Console.ReadLine());

        Console.WriteLine("Entrez le nouveau nom de l'ingrédient :");
        string nom = Console.ReadLine();
        Console.WriteLine("Entrez la nouvelle quantité :");
        string quantite = Console.ReadLine();

        string query = "UPDATE ingredients SET nom = @nom, quantite = @quantite WHERE id = @idIngredient;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@idIngredient", idIngredient);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@quantite", quantite);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
    }
    catch (FormatException)
    {
        Console.WriteLine("Erreur : Veuillez entrer un ID valide.");
    }
    finally
    {
        CloseConnection();
    }
}
public static void ModifierRecette()
{
    try
    {
        OpenConnection(); // Ouvrir la connexion

        Console.WriteLine("Entrez l'ID de la recette à modifier :");
        int idRecette = int.Parse(Console.ReadLine());

        Console.WriteLine("Entrez le nouveau titre de la recette :");
        string titre = Console.ReadLine();
        Console.WriteLine("Entrez la nouvelle description :");
        string description = Console.ReadLine();

        string query = "UPDATE recettes SET titre = @titre, description = @description WHERE id = @idRecette;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@idRecette", idRecette);
            command.Parameters.AddWithValue("@titre", titre);
            command.Parameters.AddWithValue("@description", description);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
    }
    catch (FormatException)
    {
        Console.WriteLine("Erreur : Veuillez entrer un ID valide.");
    }
    finally
    {
        CloseConnection();
    }
}
public static void ModifierPlat(int idCompte)
{
    MySqlConnection connection = null;

    try
    {
        // Configuration de la connexion à la base de données
        string connectionString = "Server=localhost;Database=livparis;User ID=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=True;";
        connection = new MySqlConnection(connectionString);
        connection.Open(); // Ouvrir la connexion

        Console.WriteLine("Entrez l'ID du plat à modifier :");
        int idPlat = int.Parse(Console.ReadLine());

        Console.WriteLine("Entrez le nouveau nom du plat :");
        string nomPlat = Console.ReadLine();
        Console.WriteLine("Entrez la nouvelle description du plat :");
        string descriptionPlat = Console.ReadLine();
        Console.WriteLine("Entrez le nouveau prix du plat :");
        decimal prixPlat = decimal.Parse(Console.ReadLine());

        // Requête SQL pour modifier le plat
        string query = "UPDATE plats SET nom = @nomPlat, description = @descriptionPlat, prix = @prixPlat WHERE id = @idPlat;";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            // Ajouter les paramètres à la requête
            command.Parameters.AddWithValue("@idPlat", idPlat);
            command.Parameters.AddWithValue("@nomPlat", nomPlat);
            command.Parameters.AddWithValue("@descriptionPlat", descriptionPlat);
            command.Parameters.AddWithValue("@prixPlat", prixPlat);

            // Exécuter la requête
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} ligne(s) modifiée(s).");
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
    }
    catch (FormatException)
    {
        Console.WriteLine("Erreur : Veuillez entrer un format valide pour le prix.");
    }
    finally
    {
        // Fermer la connexion
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}




#endregion

static public void Commander(int idClient)
{
    Program.Titre();

    if (idClient <= 0)
    {
        Console.WriteLine("ID client invalide.");
        return;
    }

    // Demander l'ID du plat
    Console.Write("Entrez l'ID du plat que vous souhaitez commander : ");
    int idPlat;
    while (!int.TryParse(Console.ReadLine(), out idPlat) || idPlat <= 0)
    {
        Console.Write("Veuillez entrer un ID de plat valide : ");
    }

    Database db = new Database();
    int idCuisinier = -1;

    try
    {
        db.OpenConnection();

        // Récupérer l'ID du cuisinier correspondant au plat
        string query = @"SELECT Id_Cuisinier FROM Plat WHERE Id_Plat = @IdPlat";
        MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
        cmd.Parameters.AddWithValue("@IdPlat", idPlat);

        object result = cmd.ExecuteScalar();

        if (result != null)
        {
            idCuisinier = Convert.ToInt32(result);
        }
        else
        {
            Console.WriteLine("Aucun cuisinier associé au plat spécifié.");
            return;
        }

        // Ajouter la commande
        int idCommande = AjouterCommande(idClient, idCuisinier);

        if (idCommande > 0)
        {
            Console.WriteLine($"La commande avec l'ID {idCommande} a été créée avec succès !");
        }
        else
        {
            Console.WriteLine("Échec de la création de la commande.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur : " + ex.Message);
    }
    finally
    {
        db.CloseConnection();
    }
}
        #region Récup id
        static public int RecupererId(int idCompte, string element)
{
    Database db = new Database();
    int idCuisinier = -1;

    try
    {
        db.OpenConnection();
        string query = "SELECT Id_"+element+" FROM " + element + " WHERE Id_Compte = @IdCompte";
        MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
        cmd.Parameters.AddWithValue("@IdCompte", idCompte);

        object result = cmd.ExecuteScalar();
        if (result != null)
        {
            idCuisinier = Convert.ToInt32(result);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur : " + ex.Message);
    }
    finally
    {
        db.CloseConnection();
    }

    return idCuisinier;
}


        #endregion
    }
}
