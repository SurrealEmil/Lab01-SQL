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
    internal class Sorting
    {

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
