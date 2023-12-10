using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL
{
    internal class MenuOptions
    {
        public static void AddNewStudents()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True; MultipleActiveResultSets=True"))
            {
                connection.Open();

                //
                int chosenClassID;
                while (true)
                {
                    Console.Clear();
                    // Display all classes for the user to choose from
                    Console.WriteLine("Välj en klass för den nya eleven genom att skriva in ett klassID:");
                    
                    using (SqlCommand getClassCommand = new SqlCommand("SELECT KlassID, KlassNamn " +
                                                                       "FROM Klasser", connection))

                    using (SqlDataReader classReader = getClassCommand.ExecuteReader())
                    {
                        while (classReader.Read())
                        {
                            Console.WriteLine($"KlassID: {classReader["KlassID"]}, Klassnamn: {classReader["KlassNamn"]}");
                        }
                    }

                    string input = Console.ReadLine();

                    if (int.TryParse(input, out chosenClassID))
                    {
                        // Validate that the chosenClassID exists in the database
                        using (SqlCommand validateClassCommand = new SqlCommand("SELECT COUNT(*) " +
                                                                                "FROM Klasser " +
                                                                                "WHERE KlassID = @ChosenClassID", connection))
                        {
                            validateClassCommand.Parameters.AddWithValue("@ChosenClassID", chosenClassID);
                            int classCount = (int)validateClassCommand.ExecuteScalar();

                            if (classCount > 0)
                            {
                                break; // Exit the loop if the input is valid
                            }
                            else
                            {
                                Console.WriteLine("Ogiltigt KlassID. Försök igen.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ogiltig inmatning. Ange ett heltal.");
                    }
                    Thread.Sleep(1000);
                }

                // Add new student
                Console.Clear();
                Console.Write("Ange förnamn för den nya eleven: ");
                string firstName = Console.ReadLine();

                Console.Clear();
                Console.Write("Ange efternamn för den nya eleven: ");
                string lastName = Console.ReadLine();

                using (SqlCommand addStudentCommand = new SqlCommand("INSERT INTO Elever (Förnamn, Efternamn, KlassID) " +
                                                                     "OUTPUT INSERTED.ElevID VALUES (@Förnamn, @Efternamn, @KlassID)", connection))
                {
                    addStudentCommand.Parameters.AddWithValue("@Förnamn", firstName);
                    addStudentCommand.Parameters.AddWithValue("@Efternamn", lastName);
                    addStudentCommand.Parameters.AddWithValue("@KlassID", chosenClassID);

                    // After inserting the new student, get the ElevID
                    int newElevID = Convert.ToInt32(addStudentCommand.ExecuteScalar());

                    // Store the ElevID for future use
                    Console.Clear();
                    Console.WriteLine($"Den nya eleven har lagts till med ElevID: {newElevID}.\n" +
                                      $"Tryck Enter för att förtsätta");
                    Console.ReadLine();

                    using (SqlCommand getCoursesCommand = new SqlCommand("SELECT KursID, KursNamn FROM Kurser", connection))
                    using (SqlDataReader courseReader = getCoursesCommand.ExecuteReader())
                    {
                        while (courseReader.Read())
                        {
                            Console.Clear();
                            // Add grades for courses
                            Console.WriteLine("Ange betyg för den nya eleven i de följande fem kurserna:\n" +
                                              "C#Basic:\n" +
                                              "SqlBasic:\n" +
                                              "HtmlBasic:\n" +
                                              "WebBasic:\n" +
                                              "DansBasic:\n" +
                                              "Betyg ska ligga mellan 0 och 100:");
                            Console.Write($"{courseReader["KursNamn"]}: ");

                            // Declare grade variable inside the loop
                            int grade;
                            bool isValidGrade;

                            do
                            {
                                // Prompt user for grade
                                string gradeInput = Console.ReadLine();

                                // Validate input as an integer
                                isValidGrade = int.TryParse(gradeInput, out grade);

                                // Check if the grade is within the valid range
                                if (grade < 0 || grade > 100)
                                {
                                    Console.WriteLine("Ogiltigt betyg. Ange ett heltal mellan 0 och 100.");
                                }
                                else
                                {
                                    break; // Exit the loop if the grade is valid
                                }
                            } while (true);

                            int currentKursID = Convert.ToInt32(courseReader["KursID"]);

                            using (SqlCommand addGradeCommand = new SqlCommand("INSERT INTO Betyg (ElevID, KursID, Betyg, Tidpunkt) " +
                                                                               "VALUES (@ElevID, @KursID, @Betyg, GETDATE())", connection))
                            {
                                addGradeCommand.Parameters.AddWithValue("@ElevID", newElevID);
                                addGradeCommand.Parameters.AddWithValue("@KursID", currentKursID);
                                addGradeCommand.Parameters.AddWithValue("@Betyg", grade);

                                addGradeCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public static void AvrageGrade()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True;"))
            {
                connection.Open();

                using (SqlCommand getCourseCommand = new SqlCommand("SELECT K.KursNamn, AVG(B.Betyg) AS AverageBetyg, MIN(B.Betyg) AS LowBetyg, MAX(B.Betyg) AS MaxBetyg " +
                                                                    "FROM Kurser K " +
                                                                    "JOIN Betyg B ON K.KursID = B.KursID " +
                                                                    "GROUP BY K.KursID, K.KursNamn", connection))

                using (SqlDataReader CourseReader = getCourseCommand.ExecuteReader())
                {
                    Console.Clear();
                    while (CourseReader.Read())
                    {
                        string kursNamn = CourseReader["KursNamn"].ToString();
                        string averageBetyg = CourseReader["AverageBetyg"].ToString();
                        string maxBetyg = CourseReader["MaxBetyg"].ToString();
                        string lowBetyg = CourseReader["LowBetyg"].ToString();

                        // Formating for better readability
                        const int kursNamnWidth = 10;
                        const int averageBetygWidth = 4;
                        const int maxBetygWidth = 4;
                        const int lowBetygWidth = 4;

                        Console.WriteLine($"Kurse: {kursNamn,-kursNamnWidth} Snittbetyg: {averageBetyg,-averageBetygWidth} Högstabetyg:: {maxBetyg,-maxBetygWidth} Lägstabetyg: {lowBetyg,-lowBetygWidth}");
                    }
                }
            }
        }

        public static void MonthlyGrade()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True;"))
            {
                connection.Open();

                DateTime lastMonth = DateTime.Now.AddMonths(-1);

                using (SqlCommand getCourseCommand = new SqlCommand("SELECT K.KursNamn, E.Förnamn + ' ' + E.Efternamn AS FullName, B.Betyg, B.Tidpunkt " +
                                                                    "FROM Kurser K " +
                                                                    "JOIN Betyg B ON K.KursID = B.KursID " +
                                                                    "JOIN Elever E ON B.ElevID = E.ElevID " +
                                                                    "WHERE B.Tidpunkt >= @LastMonth " +
                                                                    "ORDER BY FullName", connection))
                {
                    getCourseCommand.Parameters.AddWithValue("@LastMonth", lastMonth);

                    using (SqlDataReader CourseReader = getCourseCommand.ExecuteReader())
                    {
                        Console.Clear();
                        while (CourseReader.Read())
                        {
                            string kursNamn = CourseReader["KursNamn"].ToString();
                            string fullName = CourseReader["FullName"].ToString();
                            string betyg = CourseReader["Betyg"].ToString();
                            string tidpunkt = CourseReader["Tidpunkt"].ToString();

                            // Formating for better readability
                            const int kursNamnWidth = 10;
                            const int fullNameWidth = 25;
                            const int betygWidth = 4;
                            const int tidpunktWidth = 20;

                            Console.WriteLine($"Kurse: {kursNamn,-kursNamnWidth} Namn: {fullName,-fullNameWidth} Betyg: {betyg,-betygWidth} Tidpunkt: {tidpunkt,-tidpunktWidth}");
                        }
                    }
                }
            }
        }

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

        public static void GetAllStudents()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True;"))
            {
                connection.Open();

                string sortOrder = AscOrDesc();

                using (SqlCommand getCourseCommand = new SqlCommand("SELECT Förnamn, Efternamn " +
                                                                        "FROM Elever " +
                                                                        $"{sortOrder} ", connection))

                using (SqlDataReader CourseReader = getCourseCommand.ExecuteReader())
                {
                    while (CourseReader.Read())
                    {
                        Console.WriteLine($"Namn: {CourseReader["Förnamn"]} {CourseReader["Efternamn"]}");
                    }
                }
            }
        }

        public static void GetClassStudent()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\.; Initial Catalog=School; Integrated Security=True;"))
            {
                connection.Open();

                Console.WriteLine("Välj klass: \n" +
                                  "1. Net23: \n" +
                                  "2. Html23: \n" +
                                  "3. Bob23: \n" +
                                  "4. Dans23: \n" +
                                  "5. Sql23:");

                string categoryChoice = Console.ReadLine();

                string categoryFilter = string.Empty;

                switch (categoryChoice)
                {
                    case "1":
                        categoryFilter = "WHERE KlassNamn = 'Net23'";
                        break;

                    case "2":
                        categoryFilter = "WHERE KlassNamn = 'Html23'";
                        break;

                    case "3":
                        categoryFilter = "WHERE KlassNamn = 'Bob23'";
                        break;

                    case "4":
                        categoryFilter = "WHERE KlassNamn = 'Dans23'";
                        break;

                    case "5":
                        categoryFilter = "WHERE KlassNamn = 'Sql23'";
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val. Tillbaka till start meny");
                        return; // Exit the method if the choice is invalid
                }

                string sortOrder = AscOrDesc();

                using (SqlCommand getCourseCommand = new SqlCommand("SELECT E.Förnamn, E.Efternamn, K.KlassNamn " +
                                                                    "FROM Elever E " +
                                                                    "JOIN Klasser K ON E.KlassID = K.KlassID " +
                                                                    $"{categoryFilter} " +
                                                                    $"{sortOrder}", connection))

                using (SqlDataReader CourseReader = getCourseCommand.ExecuteReader())
                {
                    Console.Clear();
                    while (CourseReader.Read())
                    {
                        string klassNamn = CourseReader["KlassNamn"].ToString();

                        // Formating for better readability
                        const int klassNamnWidth = 7;
                        
                        Console.WriteLine($"Klass: {klassNamn,-klassNamnWidth} Namn: {CourseReader["Förnamn"]} {CourseReader["Efternamn"]}");
                    }
                }
            }
        }

        public static string AscOrDesc()
        {
            Console.Clear();
            Console.WriteLine("Välj sortering: \n" +
                              "1. Förnamn stigande, \n" +
                              "2. Förnamn fallande, \n" +
                              "3. Efternamn stigande, \n" +
                              "4. Efternamn fallande");

            string sortChoice = Console.ReadLine();
            Console.Clear();
            string sortOrder = string.Empty;
            switch (sortChoice)
            {
                case "1":
                    sortOrder = "ORDER BY Förnamn ASC";
                    break;
                case "2":
                    sortOrder = "ORDER BY Förnamn DESC";
                    break;
                case "3":
                    sortOrder = "ORDER BY Efternamn ASC";
                    break;
                case "4":
                    sortOrder = "ORDER BY Efternamn DESC";
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Använder standard sortering.");
                    break;
            }

            return sortOrder;
        }
    }
}
