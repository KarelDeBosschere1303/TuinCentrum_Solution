
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Exceptions;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
namespace TuinCentrumDL_SQL
{
    public class TuinCentrumRepository : ITuinCentrumRepository
    {
        private string connectionstring;
        public TuinCentrumRepository(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        public bool HeeftKlant(Klant klant)
        {
            string sql = "SELECT COUNT(*) FROM Klanten WHERE Naam = @naam";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@naam", klant.Naam);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public bool HeeftProduct(Product product)
        {
            string sql = "SELECT COUNT(*) FROM Producten WHERE Naam = @naam";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@naam", product.Naam);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public bool HeeftOfferte(Offerte offerte)
        {
            string sql = @"
            SELECT COUNT(*) 
            FROM Offertes 
            WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Id", offerte.Id);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void SchrijfKlanten(List<Klant> klanten)
        {
            string sql = "INSERT INTO Klanten (Naam, Adres) VALUES (@Naam, @Adres)";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@Adres", SqlDbType.NVarChar));
                    foreach (Klant klant in klanten)
                    {
                        command.Parameters["@Naam"].Value = klant.Naam;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SchrijfProducten(List<Product> producten)
        {
            string sql = "INSERT INTO Producten (Id,Naam, WetenschappelijkeNaam, Prijs, Beschrijving) VALUES (@Id,@Naam, @WetenschappelijkeNaam, @Prijs, @Beschrijving)";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@WetenschappelijkeNaam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@Prijs", SqlDbType.Decimal));
                    command.Parameters.Add(new SqlParameter("@Beschrijving", SqlDbType.NVarChar));
                    foreach (Product product in producten)
                    {
                      
                      
                        
                            
                            command.Parameters["@Id"].Value = product.Id;
                            command.Parameters["@Naam"].Value = product.Naam;
                            command.Parameters["@WetenschappelijkeNaam"].Value = product.WetenschappelijkeNaam;
                            command.Parameters["@Prijs"].Value = product.Prijs;
                            command.Parameters["@Beschrijving"].Value = product.Beschrijving;
                            command.ExecuteNonQuery();
                        
                    }
                }
            }
        }
        public Product GetProductById(int productId)
        {
            string sql = "SELECT * FROM Producten WHERE Id = @id";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Id", productId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() && !string.IsNullOrEmpty(reader["Naam"].ToString()))
                        {
                            Product product = new Product((int)reader["Id"],
                                                          reader["Naam"].ToString(),
                                                          reader["WetenschappelijkeNaam"].ToString(),
                                                          (decimal)reader["Prijs"],
                                                          reader["Beschrijving"].ToString());
                            return product;
                        }
                        else
                        {
                            // Handle the case when no product is found or the name is empty
                            return null;
                        }
                    }
                }
            }
        }
        public void SchrijfOffertes(List<Offerte> offertes)
        {
            string sqlInsertOfferte = @"
        INSERT INTO Offertes (Datum, KlantNummer, Afhaal, Aanleg, Kostprijs) 
        OUTPUT INSERTED.Id
        VALUES (@Datum, @KlantNummer, @Afhaal, @Aanleg, @Kostprijs)";

            string sqlInsertOfferteProducten = @"
        INSERT INTO OfferteProducten (OfferteId, ProductId, Aantal) 
        VALUES (@OfferteId, @ProductId, @Aantal)";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Offerte offerte in offertes)
                        {
                            int offerteId;
                            using (SqlCommand command = new SqlCommand(sqlInsertOfferte, connection, transaction))
                            {
                                command.Parameters.Add(new SqlParameter("@Datum", SqlDbType.DateTime)).Value = offerte.Datum;
                                command.Parameters.Add(new SqlParameter("@KlantNummer", SqlDbType.Int)).Value = offerte.Klant.Id;
                                command.Parameters.Add(new SqlParameter("@Afhaal", SqlDbType.Bit)).Value = offerte.Afhaal;
                                command.Parameters.Add(new SqlParameter("@Aanleg", SqlDbType.Bit)).Value = offerte.Aanleg;
                                command.Parameters.Add(new SqlParameter("@Kostprijs", SqlDbType.Decimal)).Value = offerte.BerekenTotaleKostPrijs();

                                offerteId = (int)command.ExecuteScalar();
                            }

                            foreach (KeyValuePair<Product, int> kvp in offerte.Producten)
                            {
                                using (SqlCommand cmdInsertProduct = new SqlCommand(sqlInsertOfferteProducten, connection, transaction))
                                {
                                    cmdInsertProduct.Parameters.Add(new SqlParameter("@OfferteId", SqlDbType.Int)).Value = offerteId;
                                    cmdInsertProduct.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int)).Value = kvp.Key.Id;
                                    cmdInsertProduct.Parameters.Add(new SqlParameter("@Aantal", SqlDbType.Int)).Value = kvp.Value;
                                    cmdInsertProduct.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        Console.WriteLine($"Inserted {offertes.Count} offertes.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw;
                    }
                }
            }
        }
        public List<Product> GetAllProducten()
        {
            List<Product> producten = new List<Product>();
            string sql = "SELECT * FROM Producten";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product((int)reader["Id"],
                                                          reader["Naam"].ToString(),
                                                          reader["WetenschappelijkeNaam"].ToString(),
                                                          (decimal)reader["Prijs"],
                                                          reader["Beschrijving"].ToString());
                            producten.Add(product);
                        }
                    }
                }
                return producten;
            }
        }


        public Dictionary<int, Klant> GetKlanten()
        {
            Dictionary<int, Klant> klantenDictionary = new Dictionary<int, Klant>();
            string sqlKlanten = "SELECT * FROM Klanten";
            string sqlOffertes = "SELECT * FROM Offertes";
            string sqlProducten = "SELECT OP.OfferteId, OP.ProductId, OP.Aantal, P.Naam, P.WetenschappelijkeNaam, P.Prijs, P.Beschrijving " +
                                  "FROM OfferteProducten OP JOIN Producten P ON OP.ProductId = P.Id";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Load all klanten
                        using (SqlCommand commandKlanten = new SqlCommand(sqlKlanten, connection, transaction))
                        {
                            using (SqlDataReader readerKlanten = commandKlanten.ExecuteReader())
                            {
                                while (readerKlanten.Read())
                                {
                                    int klantId = (int)readerKlanten["Id"];
                                    Klant klant = new Klant
                                    {
                                        Id = klantId,
                                        Naam = (string)readerKlanten["Naam"],
                                        Adres = (string)readerKlanten["Adres"],
                                        Offertes = new List<Offerte>()
                                    };
                                    klantenDictionary[klantId] = klant;
                                }
                            }
                        }

                        // Load all offertes and link to klanten
                        Dictionary<int, Offerte> offertesDictionary = new Dictionary<int, Offerte>();
                        using (SqlCommand commandOffertes = new SqlCommand(sqlOffertes, connection, transaction))
                        {
                            using (SqlDataReader readerOffertes = commandOffertes.ExecuteReader())
                            {
                                while (readerOffertes.Read())
                                {
                                    int klantNummer = (int)readerOffertes["KlantNummer"];
                                    if (klantenDictionary.ContainsKey(klantNummer))
                                    {
                                        Klant klant = klantenDictionary[klantNummer];
                                        Offerte offerte = new Offerte
                                        {
                                            Id = (int)readerOffertes["Id"],
                                            Datum = (DateTime)readerOffertes["Datum"],
                                            Afhaal = (bool)readerOffertes["Afhaal"],
                                            Aanleg = (bool)readerOffertes["Aanleg"],
                                            KostPrijs = (decimal)readerOffertes["Kostprijs"],
                                            Klant = klant,
                                            Producten = new Dictionary<Product, int>()
                                        };
                                        klant.Offertes.Add(offerte);
                                        offertesDictionary[offerte.Id] = offerte;
                                    }
                                    else
                                    {
                                        throw new Exception($"Klantnummer {klantNummer} bestaat niet in de Klanten tabel.");
                                    }
                                }
                            }
                        }

                        // Load all producten and assign them to the correct offertes
                        using (SqlCommand commandProducten = new SqlCommand(sqlProducten, connection, transaction))
                        {
                            using (SqlDataReader readerProducten = commandProducten.ExecuteReader())
                            {
                                while (readerProducten.Read())
                                {
                                    int offerteId = (int)readerProducten["OfferteId"];
                                    var product = new Product(
                                        readerProducten.GetInt32(readerProducten.GetOrdinal("ProductId")),
                                        readerProducten.GetString(readerProducten.GetOrdinal("Naam")),
                                        readerProducten.GetString(readerProducten.GetOrdinal("WetenschappelijkeNaam")),
                                        readerProducten.GetDecimal(readerProducten.GetOrdinal("Prijs")),
                                        readerProducten.GetString(readerProducten.GetOrdinal("Beschrijving"))
                                    );
                                    var aantal = readerProducten.GetInt32(readerProducten.GetOrdinal("Aantal"));

                                    if (offertesDictionary.ContainsKey(offerteId))
                                    {
                                        var offerte = offertesDictionary[offerteId];
                                        offerte.Producten.Add(product, aantal);
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error while fetching klanten and their offertes: " + ex.Message);
                    }
                }
            }

            return klantenDictionary;
        }


        public void AddOfferte(Offerte offerte)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Offertes (KlantNummer, Datum, Afhaal, Aanleg, Kostprijs) OUTPUT INSERTED.Id VALUES (@KlantNummer, @Datum, @Afhaal, @Aanleg, @Kostprijs)", connection))
                {
                    command.Parameters.AddWithValue("@KlantNummer", offerte.Klant.Id);
                    command.Parameters.AddWithValue("@Datum", offerte.Datum);
                    command.Parameters.AddWithValue("@Afhaal", offerte.Afhaal);
                    command.Parameters.AddWithValue("@Aanleg", offerte.Aanleg);
                    command.Parameters.AddWithValue("@Kostprijs", offerte.KostPrijs);

                    offerte.Id = (int)command.ExecuteScalar();
                }

                foreach (var product in offerte.Producten)
                {
                    using (SqlCommand insertCommand = new SqlCommand("INSERT INTO OfferteProducten (OfferteId, ProductId, Aantal) VALUES (@OfferteId, @ProductId, @Aantal)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@OfferteId", offerte.Id);
                        insertCommand.Parameters.AddWithValue("@ProductId", product.Key.Id);
                        insertCommand.Parameters.AddWithValue("@Aantal", product.Value);

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateOfferte(Offerte offerte)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();

                // Update de offertegegevens
                using (SqlCommand command = new SqlCommand("UPDATE Offertes SET KlantNummer = @KlantNummer, Datum = @Datum, Afhaal = @Afhaal, Aanleg = @Aanleg, Kostprijs = @Kostprijs WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", offerte.Id);
                    command.Parameters.AddWithValue("@KlantNummer", offerte.Klant.Id);
                    command.Parameters.AddWithValue("@Datum", offerte.Datum);
                    command.Parameters.AddWithValue("@Afhaal", offerte.Afhaal);
                    command.Parameters.AddWithValue("@Aanleg", offerte.Aanleg);
                    command.Parameters.AddWithValue("@Kostprijs", offerte.KostPrijs);

                    command.ExecuteNonQuery();
                }

                // Verwijder bestaande producten van de offerte
                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM OfferteProducten WHERE OfferteId = @OfferteId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@OfferteId", offerte.Id);
                    deleteCommand.ExecuteNonQuery();
                }

                // Voeg de bijgewerkte producten opnieuw toe
                foreach (var product in offerte.Producten)
                {
                    using (SqlCommand insertCommand = new SqlCommand("INSERT INTO OfferteProducten (OfferteId, ProductId, Aantal) VALUES (@OfferteId, @ProductId, @Aantal)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@OfferteId", offerte.Id);
                        insertCommand.Parameters.AddWithValue("@ProductId", product.Key.Id);
                        insertCommand.Parameters.AddWithValue("@Aantal", product.Value);

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public Offerte GetOfferteSById(int offerteId)
        {
            Offerte offerte = null;
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Offertes WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", offerteId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int klantId = (int)reader["KlantNummer"];
                            Klant klant = GetKlantById(klantId);

                            offerte = new Offerte
                            {
                                Id = (int)reader["Id"],
                                Klant = klant,
                                Datum = (DateTime)reader["Datum"],
                                Afhaal = (bool)reader["Afhaal"],
                                Aanleg = (bool)reader["Aanleg"],
                                KostPrijs = (decimal)reader["Kostprijs"],
                                Producten = new Dictionary<Product, int>()
                            };
                        }
                    }
                }

                if (offerte != null)
                {
                    using (SqlCommand command = new SqlCommand("SELECT OP.ProductId, OP.Aantal, P.Naam, P.WetenschappelijkeNaam, P.Prijs, P.Beschrijving FROM OfferteProducten OP JOIN Producten P ON OP.ProductId = P.Id WHERE OfferteId = @OfferteId", connection))
                    {
                        command.Parameters.AddWithValue("@OfferteId", offerteId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product(
                                    reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    reader.GetString(reader.GetOrdinal("Naam")),
                                    reader.GetString(reader.GetOrdinal("WetenschappelijkeNaam")),
                                    reader.GetDecimal(reader.GetOrdinal("Prijs")),
                                    reader.GetString(reader.GetOrdinal("Beschrijving"))
                                );
                                var aantal = reader.GetInt32(reader.GetOrdinal("Aantal"));
                                offerte.Producten.Add(product, aantal);
                            }
                        }
                    }
                }
            }
            return offerte;
        }

        public Klant GetKlantById(int klantId)
        {
            Klant klant = null;
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string sql = "SELECT * FROM Klanten WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", klantId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            klant = new Klant
                            {
                                Id = (int)reader["Id"],
                                Naam = (string)reader["Naam"],
                                Adres = (string)reader["Adres"]
                            };
                        }
                    }
                }
            }
            return klant;
        }
        public void AddKlant(Klant klant)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO Klanten (Id, Naam, Adres) VALUES (@Id, @Naam, @Adres)", connection))
                {
                    command.Parameters.AddWithValue("@Id", klant.Id);
                    command.Parameters.AddWithValue("@Naam", klant.Naam);
                    command.Parameters.AddWithValue("@Adres", klant.Adres);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}

