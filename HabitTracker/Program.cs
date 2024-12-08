using System.Globalization;
using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";
        static void Main(string[] args)
        {
            

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                        )";
                
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
            
            GetUserInput();
        }
        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;

            while (!closeApp)
            {
                Console.WriteLine("\n\nMain Menu");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("Type 0 to close application");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Insert Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("--------------------------------------\n");
                    
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    //     case "3":
                    //         Delete();
                    //         break;
                    //     case "4":
                    //         Update();
                    //         break;
                    //     default:
                    //         Console.WriteLine("Invalid command. Please try again.");
                    // }
                }
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();
                
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.hasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater()
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo(("en-US"))),
                                Quantity = reader.GetInt32(2),
                            });
                    }
                }
                
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        
        private static void Insert()
        {
            string date = GetDateInput();
                        
            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO drinking_water (Date, Quantity) VALUES ('{date}', {quantity})";
                
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu");
                        
            string dateInput = Console.ReadLine();
                        
            if (dateInput == "0") GetUserInput();
                        
            return dateInput;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
                        
            string numberInput = Console.ReadLine();
                        
            if (numberInput == "0") GetUserInput();
                        
            int finalInput = Convert.ToInt32(numberInput);
                        
            return finalInput;
        }

        public class DrinkingWater
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
        }
    }
}