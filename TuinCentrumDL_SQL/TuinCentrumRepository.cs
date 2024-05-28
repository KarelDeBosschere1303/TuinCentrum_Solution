
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
                    WHERE Datum = @Datum 
                      AND KlantNummer = @KlantNummer
                      AND Afhaal = @Afhaal
                      AND Aanleg = @Aanleg
                      AND Kostprijs = @Kostprijs";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Datum", offerte.Datum.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@KlantNummer", offerte.KlantNummer);
                    command.Parameters.AddWithValue("@Afhaal", offerte.Afhaal);
                    command.Parameters.AddWithValue("@Aanleg", offerte.Aanleg);
                    command.Parameters.AddWithValue("@Kostprijs", offerte.BerekenTotaleKostPrijs());

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
                        command.Parameters["@Adres"].Value = klant.Adres;
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
                        // Check if a product with the same ID already exists
                        SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Producten WHERE Id = @Id", connection);
                        checkCommand.Parameters.AddWithValue("@Id", product.Id);
                        int existingCount = (int)checkCommand.ExecuteScalar();

                        if (existingCount == 0)
                        {
                            // Only insert the product if it does not already exist
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
                    VALUES (@Datum, (SELECT Id FROM Klanten WHERE Id = @KlantNummer), @Afhaal, @Aanleg, @Kostprijs)";

            string sqlInsertOfferteProducten = @"
                    INSERT INTO OfferteProducten (OfferteId, ProductId, Aantal) 
                    VALUES (@OfferteId, @ProductId, @Aantal)";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();

                foreach (Offerte offerte in offertes)
                {
                    int offerteId;
                    using (SqlCommand command = new SqlCommand(sqlInsertOfferte, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Datum", SqlDbType.DateTime)).Value = offerte.Datum.ToString("yyyy-MM-dd");
                        command.Parameters.Add(new SqlParameter("@KlantNummer", SqlDbType.Int)).Value = offerte.KlantNummer;
                        command.Parameters.Add(new SqlParameter("@Afhaal", SqlDbType.Bit)).Value = offerte.Afhaal;
                        command.Parameters.Add(new SqlParameter("@Aanleg", SqlDbType.Bit)).Value = offerte.Aanleg;
                        command.Parameters.Add(new SqlParameter("@Kostprijs", SqlDbType.Decimal)).Value = offerte.BerekenTotaleKostPrijs();

                        // Verkrijg het gegenereerde OfferteId
                        offerteId = (int)command.ExecuteScalar();
                    }

                    // Voeg de producten voor de offerte in
                    foreach (KeyValuePair<Product, int> kvp in offerte.Producten)
                    {
                        if (kvp.Key != null)
                        {
                            using (SqlCommand cmdInsertProduct = new SqlCommand(sqlInsertOfferteProducten, connection))
                            {
                                cmdInsertProduct.Parameters.Add(new SqlParameter("@OfferteId", SqlDbType.Int)).Value = offerteId;
                                cmdInsertProduct.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int)).Value = kvp.Key.Id;
                                cmdInsertProduct.Parameters.Add(new SqlParameter("@Aantal", SqlDbType.Int)).Value = kvp.Value;
                                cmdInsertProduct.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Log the count of inserted offertes
                Console.WriteLine($"Inserted {offertes.Count} offertes.");
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
        public List<Klant> GetKlanten()
        {
            List<Klant> klanten = new List<Klant>();
            string sql = "SELECT * FROM Klanten";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Klant klant = new Klant((int)reader["Id"],
                                                      reader["Naam"].ToString(),
                                                      reader["Adres"].ToString());
                            klanten.Add(klant);
                        }
                    }
                }
                return klanten;
            }
        }

        public List<Offerte> GetOffertes()
        {
            List<Offerte> offertes = new List<Offerte>();
            string sql = "SELECT * FROM Offertes";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Offerte offerte = new Offerte((int)reader["Id"],
                                                          (DateTime)reader["Datum"],
                                                          (int)reader["KlantNummer"],
                                                          (bool)reader["Afhaal"],
                                                          (bool)reader["Aanleg"],
                                                          (decimal)reader["Kostprijs"]);
                            offertes.Add(offerte);
                        }
                    }
                }
                return offertes;
            }
        }
        public void AddOfferte(Offerte offerte)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Offertes (KlantNummer, Datum, Afhaal, Aanleg, Kostprijs) OUTPUT INSERTED.Id VALUES (@KlantNummer, @Datum, @Afhaal, @Aanleg, @Kostprijs)", connection))
                {
                    command.Parameters.AddWithValue("@KlantNummer", offerte.KlantNummer);
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
                using (SqlCommand command = new SqlCommand("UPDATE Offertes SET KlantNummer = @KlantNummer, Datum = @Datum, Afhaal = @Afhaal, Aanleg = @Aanleg, Kostprijs = @Kostprijs WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", offerte.Id);
                    command.Parameters.AddWithValue("@KlantNummer", offerte.KlantNummer);
                    command.Parameters.AddWithValue("@Datum", offerte.Datum);
                    command.Parameters.AddWithValue("@Afhaal", offerte.Afhaal);
                    command.Parameters.AddWithValue("@Aanleg", offerte.Aanleg);
                    command.Parameters.AddWithValue("@Kostprijs", offerte.KostPrijs);

                    command.ExecuteNonQuery();
                }

                // Delete existing products and re-insert the updated ones
                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM OfferteProducten WHERE OfferteId = @OfferteId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@OfferteId", offerte.Id);
                    deleteCommand.ExecuteNonQuery();
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
        public Offerte GetOfferteById(int offerteId)
        {
            Offerte offerte = null;
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                // Haal de offerte gegevens op
                using (SqlCommand command = new SqlCommand("SELECT * FROM Offertes WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", offerteId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            offerte = new Offerte
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                KlantNummer = reader.GetInt32(reader.GetOrdinal("KlantNummer")),
                                Datum = reader.GetDateTime(reader.GetOrdinal("Datum")),
                                Afhaal = reader.GetBoolean(reader.GetOrdinal("Afhaal")),
                                Aanleg = reader.GetBoolean(reader.GetOrdinal("Aanleg")),
                                KostPrijs = reader.GetDecimal(reader.GetOrdinal("Kostprijs"))
                            };
                        }
                    }
                }

                // Haal de producten van de offerte op
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
    }
}

