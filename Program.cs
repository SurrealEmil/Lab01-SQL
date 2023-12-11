using System.Data.SqlClient;

namespace SQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            NewDatabase.AddKlassAndKurs();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Skriv in ett nummer mellan 1 till 8:");
                Console.WriteLine("1: Hämta alla elever");
                Console.WriteLine("2: Hämta alla elever i en vise klass");
                Console.WriteLine("3: Lägg till ny personal");
                Console.WriteLine("4: Hämta personal");
                Console.WriteLine("5: Hämta alla betyg som satt den senaste månaden");
                Console.WriteLine("6: Snitt, högsta och lägsta betygen per kurs");
                Console.WriteLine("7: Lägg till nya elever");
                Console.WriteLine("8: Avsluta programmet");

                int userInput = int.Parse(Console.ReadLine());
                Console.Clear();

                switch (userInput)
                {
                    case 1:
                        StudentMethods.GetAllStudents();
                        break;
                    case 2:
                        StudentMethods.GetClassStudent();
                        break;
                    case 3:
                        StaffMethods.AddNewWorker();
                        break;
                    case 4:
                        StaffMethods.GetWorker();
                        break;
                    case 5:
                        StudentMethods.MonthlyGrade();
                        break;
                    case 6:
                        StudentMethods.AvrageGrade();
                        break;
                    case 7:
                        StudentMethods.AddNewStudents();
                        break;
                    case 8:
                        Console.WriteLine("Programmet avslutas...");
                        Thread.Sleep(650);
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Ogiltig inmatning. Ange ett nummer mellan 1 och 8.");
                        break;
                }

                Console.WriteLine("Tryck Enter för att komma tilbaka till meny");
                Console.ReadLine();
            }
        }
    }
}