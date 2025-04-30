-- Creer la base de donnees et les tables
DROP DATABASE IF EXISTS livparis;
CREATE DATABASE livparis;
USE livparis;

-- Creation des tables

CREATE TABLE IF NOT EXISTS Compte (
   Id_Compte INT AUTO_INCREMENT,
   Prenom VARCHAR(50),
   Nom VARCHAR(50),
   Rue VARCHAR(50),
   Numero INT,
   Code_postal INT,
   Ville VARCHAR(50),
   No_tel VARCHAR(20),
   Email VARCHAR(50),
   Station_de_Metro_la_plus_Proche VARCHAR(50),
   Statut VARCHAR(50),   
   Mot_Passe VARCHAR(50),
   PRIMARY KEY(Id_Compte)
);

CREATE TABLE IF NOT EXISTS Client(
   Id_Client INT AUTO_INCREMENT,
   Nom_Entreprise VARCHAR(50),
   Id_Compte INT NOT NULL,
   PRIMARY KEY(Id_Client),
   UNIQUE(Id_Compte),
   FOREIGN KEY(Id_Compte) REFERENCES Compte(Id_Compte) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Recette(
   Id_Recette INT AUTO_INCREMENT,
   Nom_Recette VARCHAR(50),
   Instructions VARCHAR(50),
   Temps_Preparation INT,
   Temps_Cuisson INT,
   Difficulte VARCHAR(50),
   PRIMARY KEY(Id_Recette)
);

CREATE TABLE IF NOT EXISTS Ingredient(
   Id_Ingredient INT AUTO_INCREMENT,
   Volume INT,
   Ingredient VARCHAR(50),
   PRIMARY KEY(Id_Ingredient)
);

CREATE TABLE IF NOT EXISTS Cuisinier(
   Id_Cuisinier INT AUTO_INCREMENT,
   Zone_Livraison VARCHAR(50),
   Id_Compte INT NOT NULL,
   PRIMARY KEY(Id_Cuisinier),
   UNIQUE(Id_Compte),
   FOREIGN KEY(Id_Compte) REFERENCES Compte(Id_Compte) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Plat(
   Id_Plat INT AUTO_INCREMENT,
   Type_Plat VARCHAR(50),
   Date_Peremption DATE,
   Date_Fabrication DATE,
   Type_Regime VARCHAR(50),
   Photo VARCHAR(50),
   Description VARCHAR(50),
   Nationalite VARCHAR(50),
   Prix DECIMAL(15,2),
   Nombre_Portion INT,
   Ingredients_Principaux VARCHAR(50),
   Id_Recette INT,
   Id_Cuisinier INT NOT NULL,
   PRIMARY KEY(Id_Plat),
   FOREIGN KEY(Id_Recette) REFERENCES Recette(Id_Recette) ON DELETE SET NULL,
   FOREIGN KEY(Id_Cuisinier) REFERENCES Cuisinier(Id_Cuisinier) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Avis(
   Id_Avis INT AUTO_INCREMENT,
   Note INT,
   Date_Avis DATE,
   Commentaire VARCHAR(50),
   Id_Client INT NOT NULL,
   Id_Cuisinier INT NOT NULL,
   PRIMARY KEY(Id_Avis),
   FOREIGN KEY(Id_Client) REFERENCES Client(Id_Client) ON DELETE CASCADE,
   FOREIGN KEY(Id_Cuisinier) REFERENCES Cuisinier(Id_Cuisinier) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Commande(
   Id_Commande INT AUTO_INCREMENT,
   Date_Commande DATE,
   Statut_Commande VARCHAR(50),
   Prix_Total DECIMAL(15,2),
   Statut_Transaction VARCHAR(50),
   Date_Paiement DATE,
   Mode_Paiement VARCHAR(50),
   Id_Client INT NOT NULL,
   Id_Cuisinier INT NOT NULL,
   PRIMARY KEY(Id_Commande),
   FOREIGN KEY(Id_Client) REFERENCES Client(Id_Client) ON DELETE CASCADE,
   FOREIGN KEY(Id_Cuisinier) REFERENCES Cuisinier(Id_Cuisinier) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS contient(
   Id_Recette INT,
   Id_Ingredient INT,
   PRIMARY KEY(Id_Recette, Id_Ingredient),
   FOREIGN KEY(Id_Recette) REFERENCES Recette(Id_Recette) ON DELETE CASCADE,
   FOREIGN KEY(Id_Ingredient) REFERENCES Ingredient(Id_Ingredient) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS comprend(
   Id_Commande INT,
   Id_Plat INT,
   PRIMARY KEY(Id_Commande, Id_Plat),
   FOREIGN KEY(Id_Commande) REFERENCES Commande(Id_Commande) ON DELETE CASCADE,
   FOREIGN KEY(Id_Plat) REFERENCES Plat(Id_Plat) ON DELETE CASCADE
);

INSERT INTO Compte (Prenom, Nom, Rue, Numero, Code_postal, Ville, No_tel, Email, Station_de_Metro_la_plus_Proche, Statut, Mot_Passe)
VALUES
('Jean',     'Dupont',    'Rue de la Paix',           10, 75002, 'Paris',            '0102030405', 'jean.dupont@example.com',         'Opera',                'Actif',   'pass1'),
('Sophie',   'Martin',    'Av. des Champs-elysees',   50, 75008, 'Paris',            '0607080910', 'sophie.martin@example.com',      'Franklin D. Roosevelt','Inactif', 'pass2'),
('Claude',   'Bernard',   'Rue de la Republique',     15, 69002, 'Lyon',             '0708091011', 'claude.bernard@lyon.fr',         'Bellecour',            'Actif',   'pass3'),
('Alice',    'Durand',    'Cours Mirabeau',            5, 13100, 'Aix-en-Provence',  '0809101112', 'alice.durand@aix.fr',            'Rotonde',              'Actif',   'pass4'),
('Paul',     'Petit',     'Rue de Strasbourg',         8, 67000, 'Strasbourg',       '0910111213', 'paul.petit@strasbourg.fr',       'Homme de Fer',         'Inactif', 'pass5'),
('Claire',   'Leroy',     'Boulevard Voltaire',       20, 75011, 'Paris',            '0123456789', 'claire.leroy@example.com',       'Nation',               'Actif',   'pass6'),
('Thomas',   'Rousseau',  'Place de la Bastille',     12, 75004, 'Paris',            '0234567890', 'thomas.rousseau@example.com',    'Bastille',             'Actif',   'pass7'),
('elodie',   'Faure',     'Rue Mouffetard',           34, 75005, 'Paris',            '0345678901', 'elodie.faure@example.com',       'Place Monge',          'Actif',   'pass8'),
('Marc',     'Lefèvre',   'Av. Jean Jaurès',          78, 92110, 'Clichy',           '0456789012', 'marc.lefevre@example.com',       'Mairie de Clichy',     'Inactif', 'pass9'),
('Laura',    'Moreau',    'Rue de Rennes',            42, 75006, 'Paris',            '0567890123', 'laura.moreau@example.com',       'Saint-Germain-des-Pres','Actif', 'pass10');

INSERT INTO Client (Nom_Entreprise, Id_Compte)
VALUES
('Dupont SARL',        1),
('Martin & Co',        2),
('Bernard Industries', 3),
('Durand & Fils',      4),
('Petit Traiteur',     5),
('Leroy Digital',      6),
('Rousseau Tech',      7);

INSERT INTO Cuisinier (Zone_Livraison, Id_Compte)
VALUES
('Paris-centre',  2),
('Ouest Parisien','3'),
('Sud-Est',       4),
('Nord',          5),
('Banlieue Ouest',7),
('Banlieue Est',  8),
('Grand Paris',   9);

INSERT INTO Recette (Nom_Recette, Instructions, Temps_Preparation, Temps_Cuisson, Difficulte)
VALUES
('Tarte aux pommes', 'eplucher et trancher les pommes, disposer sur pâte, cuire', 20, 30, 'Moyen'),
('Salade niçoise',   'Melanger legumes, œufs, thon, assaisonner',             15, 0,  'Facile'),
('Quiche lorraine',  'Preparer la pâte, garnir œufs-crème-lardons, cuire',    25, 35, 'Moyen'),
('Mousse au chocolat','Faire fondre chocolat, monter blanc en neige, melanger', 10, 0, 'Difficile'),
('Soupe à l''oignon','Faire revenir oignons, mouiller, gratiner',           30, 20, 'Moyen'),
('Couscous vegetarien','Cuire semoule, legumes, epices, assembler',        40, 30, 'Moyen');


INSERT INTO Ingredient (Volume, Ingredient)
VALUES
(200, 'Pommes'),
(100, 'Farine'),
(150, 'Lait'),
(50,  'Beurre'),
(100, 'Salade'),
(80,  'Thon'),
(70,  'Lardons'),
(60,  'Chocolat'),
(40,  'Oignons'),
(120, 'Semoule'),
(90,  'Legumes varies'),
(30,  'Œufs');

INSERT INTO contient (Id_Recette, Id_Ingredient)
VALUES
(1, 1),(1,2),(1,4),        -- Tarte aux pommes : Pommes, Farine, Beurre
(2, 5),(2,6),(2,12),       -- Salade niçoise : Salade, Thon, Œufs
(3, 2),(3,7),(3,12),       -- Quiche lorraine : Farine, Lardons, Œufs
(4, 3),(4,8),(4,12),       -- Mousse chocolat : Lait, Chocolat, Œufs
(5, 9),(5,4),(5,2),        -- Soupe à l'oignon : Oignons, Beurre, Farine
(6,10),(6,11),(6,3);       -- Couscous vegetarien : Semoule, Legumes, Lait


INSERT INTO Plat (
Type_Plat, Date_Peremption, Date_Fabrication, Type_Regime,
Photo, Description, Nationalite, Prix, Nombre_Portion,
`Ingredients_Principaux`, Id_Recette, Id_Cuisinier
)
VALUES
('Dessert', '2025-05-05','2025-04-29','Standard','tarte.jpg','Tarte aux pommes maison','Française',  14.00, 4, 'Pommes', 1,  2),
('Entree',  '2025-05-03','2025-04-30','Vegetarien','salade.jpg','Salade niçoise fraîche', 'Française', 8.50,  2, 'Laitue, Thon', 2,  1),
('Plat',    '2025-05-06','2025-05-01','Standard','quiche.jpg','Quiche lorraine traditionnelle','Française', 12.00, 4, 'Lardons', 3,  3),
('Dessert', '2025-05-04','2025-04-28','Sans sucre','mousse.jpg','Mousse au chocolat intense','Française', 6.50,  1, 'Chocolat', 4,  4),
('Entree',  '2025-05-07','2025-05-02','Vegetalien','soupe.jpg','Soupe à l’oignon gratinee','Française', 7.00,  3, 'Oignons', 5,  5),
('Plat',    '2025-05-08','2025-05-03','Vegetarien','couscous.jpg','Couscous legumes','Internationale', 10.00, 4, 'Semoule', 6,  6);


INSERT INTO Commande (
Date_Commande, Statut_Commande, Prix_Total,
Statut_Transaction, Date_Paiement, Mode_Paiement,
Id_Client, Id_Cuisinier
)
VALUES
('2025-04-28','En cours',        28.00,'Paye','2025-04-28','CB',      1, 2),
('2025-04-29','Livree',         17.00,'Paye','2025-04-29','Espèces',2, 1),
('2025-04-29','Annulee',        12.00,'Non paye',NULL,        'CB',      3, 3),
('2025-04-30','En preparation',  6.50,'Paye','2025-04-30','PayPal', 4, 4),
('2025-05-01','Livree',         10.00,'Paye','2025-05-01','CB',      5, 5),
('2025-05-01','En cours',       14.00,'Non paye',NULL,        'Espèces',6, 2);


INSERT INTO comprend (Id_Commande, Id_Plat)
VALUES
(1,1),(1,2),
(2,3),
(3,2),(3,5),
(4,4),
(5,6),
(6,1),(6,3);


INSERT INTO Avis (Note, Date_Avis, Commentaire, Id_Client, Id_Cuisinier)
VALUES
(5,'2025-04-29','Delicieux, merci !',    1,2),
(4,'2025-04-29','Bon, un peu trop sale', 2,1),
(3,'2025-04-30','Correct',             3,3),
(5,'2025-04-30','Excellent plat !',    4,4),
(4,'2025-05-01','Savoureux',           5,5),
(5,'2025-05-01','Fan de legumes',      6,6);
