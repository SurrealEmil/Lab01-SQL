using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL
{
    internal class NewDatabase
    {
        // In case of new database classes and courses will be added
        public static void AddKlassAndKurs()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True"))
            {
                connection.Open();

                using (SqlCommand countCommand = new SqlCommand("SELECT COUNT(*) " +
                                                                "FROM Klasser", connection))
                {
                    int rowCount = (int)countCommand.ExecuteScalar();

                    if (rowCount == 0)
                    {

                        using (SqlCommand command = new SqlCommand("INSERT INTO Klasser (KlassNamn) " +
                                                                   "VALUES (@KlassNamn)", connection))
                        {
                            // Add a parameter
                            command.Parameters.Add("@KlassNamn", SqlDbType.VarChar);

                            // Iterate over the class names and execute the query for each
                            List<string> klassNamn = new List<string> { "Net23", "Html23", "Bob23", "Dans23", "Sql23" };

                            foreach (string className in klassNamn)
                            {
                                command.Parameters["@KlassNamn"].Value = className;
                                command.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand command = new SqlCommand("INSERT INTO Kurser (KursNamn) " +
                                                                   "VALUES (@KursNamn)", connection))
                        {
                            // Add a parameter
                            command.Parameters.Add("@KursNamn", SqlDbType.VarChar);

                            // Iterate over the course names and execute the query for each
                            List<string> KursNamn = new List<string> { "C#Basic", "SqlBasic", "HtmlBasic", "WebBasic", "DansBasic" };

                            foreach (string CourseName in KursNamn)
                            {
                                command.Parameters["@KursNamn"].Value = CourseName;
                                command.ExecuteNonQuery();
                            }
                        }

                        Console.WriteLine("Data inserted into Klasser and Kurser table.");
                    }
                    else
                    {
                        Console.WriteLine("Klasser and kurser table is not empty. No data inserted.");
                    }
                }
            }
            Console.WriteLine("Tryck på Enter för att komma igång");
            Console.ReadLine();
        }
    }
}
