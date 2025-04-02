using MySql.Data.MySqlClient;
using PSI_RENDU1;
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
        static public int CreationCompte()
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

            Console.Write("Statut : ");
            string statut = Console.ReadLine();

            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine(); // Il serait préférable de le hasher avant de le stocker.

            int idCompte = -1;

            try
            {
                db.OpenConnection();
                string query = "INSERT INTO Compte (Prenom, Nom, Rue, Numero, Code_postal, Ville, No_tel, Email, Station_de_Métro_la_plus_Proche, Statut, Mot_Passe) " +
                               "VALUES (@Prenom, @Nom, @Rue, @Numero, @CodePostal, @Ville, @Telephone, @Email, @StationMetro, @Statut, @MotPasse)";

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
                cmd.Parameters.AddWithValue("@Statut", statut);
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
        static public void CreationClient(int idCompte)
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
        static public void CreationCuisinier(int idCompte)
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
        static public void CreationRecette()
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

            Console.Write("Difficulté (Facile/Moyen/Difficile) : ");
            string difficulte = Console.ReadLine();

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
        static public void CreationIngredient()
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
        static public void CreationPlat(int idCuisinier)
        {
            Database db = new Database();

            Console.WriteLine("Ajout d'un nouveau plat\n");

            Console.Write("Type de plat : ");
            string typePlat = Console.ReadLine();

            Console.Write("Date de fabrication (YYYY-MM-DD) : ");
            string dateFabrication = Console.ReadLine();

            Console.Write("Date de péremption (YYYY-MM-DD) : ");
            string datePeremption = Console.ReadLine();

            Console.Write("Type de régime (ex: Végétarien, Sans gluten) : ");
            string typeRegime = Console.ReadLine();

            Console.Write("Photo (URL ou nom du fichier) : ");
            string photo = Console.ReadLine();

            Console.Write("Description : ");
            string description = Console.ReadLine();

            Console.Write("Nationalité : ");
            string nationalite = Console.ReadLine();

            Console.Write("Prix (€) : ");
            decimal prix;
            while (!decimal.TryParse(Console.ReadLine(), out prix) || prix < 0)
            {
                Console.Write("Veuillez entrer un prix valide : ");
            }

            Console.Write("Nombre de portions : ");
            int nombrePortion;
            while (!int.TryParse(Console.ReadLine(), out nombrePortion) || nombrePortion < 0)
            {
                Console.Write("Veuillez entrer un nombre valide : ");
            }

            Console.Write("Ingrédients principaux : ");
            string ingredientsPrincipaux = Console.ReadLine();

            Console.Write("ID de la recette (si disponible, sinon 0) : ");
            int idRecette;
            while (!int.TryParse(Console.ReadLine(), out idRecette))
            {
                Console.Write("Veuillez entrer un ID valide : ");
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
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        static public int CreationCommande(int idClient, int idCuisinier)
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
                    int idCommande = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine("Commande créée avec succès. ID de la commande : " + idCommande);
                    return idCommande;
                }
                else
                {
                    Console.WriteLine("Échec de la création de la commande.");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return -1;
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
                CreationClient(idCompte);
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
                        CreationClient(idCompte);
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
                CreationCuisinier(idCompte);
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
                        CreationCuisinier(idCompte);
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
        static public void SupprimerCuisinier()
        {
            Database db = new Database();

            Console.Write("Entrez l'ID du cuisinier à supprimer : ");
            int idClient;
            while (!int.TryParse(Console.ReadLine(), out idClient))
            {
                Console.Write("Veuillez entrer un ID valide : ");
            }

            Console.Write("Êtes-vous sûr de vouloir supprimer ce cuisinier ? (O/N) : ");
            string confirmation = Console.ReadLine();
            if (confirmation.ToLower() != "o")
            {
                Console.WriteLine("Suppression annulée.");
                return;
            }

            try
            {
                db.OpenConnection();
                string query = "DELETE FROM Client WHERE Id_Client = @IdClient";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.Parameters.AddWithValue("@IdClient", idClient);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("Cuisinier supprimé avec succès !");
                else
                    Console.WriteLine("Aucun cuisinier trouvé avec cet ID.");
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
        static public void SupprimerCompte()
        {

        }// à faire


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
    }
}

