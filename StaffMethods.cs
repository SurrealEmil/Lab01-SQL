using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL
{
    internal class StaffMethods
    {
        public static void AddNewWorker()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True"))
            {
                connection.Open();

                Console.WriteLine("Välj en kategori för den nya personalen (1-5):\n" +
                                  "1. Lärare\n" +
                                  "2. Rector\n" +
                                  "3. Bibliotekarie\n" +
                                  "4. Vaktmästare\n" +
                                  "5. Administratör");

                string categoryChoice = Console.ReadLine();

                string categoryFilter = string.Empty;

                switch (categoryChoice)
                {
                    case "1":
                        categoryFilter = "Lärare";
                        break;

                    case "2":
                        categoryFilter = "Rector";
                        break;

                    case "3":
                        categoryFilter = "Bibliotekarie";
                        break;

                    case "4":
                        categoryFilter = "Vaktmästare";
                        break;

                    case "5":
                        categoryFilter = "Administratör";
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val. Tillbaka till start meny");
                        return; // Exit the method if the choice is invalid
                }

                Console.Clear();
                Console.Write("Skriv in förnamn för ny personal: ");
                string firstName = Console.ReadLine();

                Console.Clear();
                Console.Write("Skriv in efternamn för ny personal: ");
                string lastName = Console.ReadLine();

                using (SqlCommand addWorkerCommand = new SqlCommand("INSERT INTO Personal (Förnamn, Efternamn, Kategori) " +
                                                                    "OUTPUT INSERTED.PersonalID VALUES (@Förnamn, @Efternamn, @Kategori)", connection))

                {
                    addWorkerCommand.Parameters.AddWithValue("@Förnamn", firstName);
                    addWorkerCommand.Parameters.AddWithValue("@Efternamn", lastName);
                    addWorkerCommand.Parameters.AddWithValue("@Kategori", categoryFilter);

                    // Execute the SQL command
                    int insertedId = (int)addWorkerCommand.ExecuteScalar();

                    Console.Clear();
                    Console.WriteLine($"Ny personal inlagd med ID: {insertedId}");
                }

            }
        }

        public static void GetWorker()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True;"))
            {
                connection.Open();

                Console.WriteLine("Välj en kategori för den nya personalen: Mellan 1 till 6:\n" +
                                  "1. Lärare\n" +
                                  "2. Rector\n" +
                                  "3. Bibliotekarie\n" +
                                  "4. Vaktmästare\n" +
                                  "5. Administratör\n" +
                                  "6. Alla kategorier");

                string categoryChoice = Console.ReadLine();

                string categoryFilter = string.Empty;

                switch (categoryChoice)
                {
                    case "1":
                        categoryFilter = "WHERE Kategori = 'Lärare'";
                        break;

                    case "2":
                        categoryFilter = "WHERE Kategori = 'Rector'";
                        break;

                    case "3":
                        categoryFilter = "WHERE Kategori = 'Bibliotekarie'";
                        break;

                    case "4":
                        categoryFilter = "WHERE Kategori = 'Vaktmästare'";
                        break;

                    case "5":
                        categoryFilter = "WHERE Kategori = 'Administratör'";
                        break;

                    case "6":
                        // No filter for "Alla kategorier"
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val. Tillbaka till start meny");
                        return; // Exit the method if the choice is invalid
                }

                using (SqlCommand getCourseCommand = new SqlCommand("SELECT Kategori, Förnamn + ' ' + Efternamn AS FullName " +
                                                                    "FROM Personal " +
                                                                    $"{categoryFilter} " +
                                                                    "ORDER BY FullName", connection))

                using (SqlDataReader CourseReader = getCourseCommand.ExecuteReader())
                {
                    Console.Clear();
                    while (CourseReader.Read())
                    {
                        string fullName = CourseReader["FullName"].ToString();
                        string jobCategory = CourseReader["Kategori"].ToString();

                        // Formating for better readability
                        const int fullNameWidth = 22;
                        const int jobCategoryWidth = 15;

                        Console.WriteLine($"Namn: {fullName,-fullNameWidth}Jobb: {jobCategory,-jobCategoryWidth}");
                    }
                }

            }
        }
    }
}
