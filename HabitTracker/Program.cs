using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Habit_Tracker
{
    class Program
    {
        static string connectionString = "Data Source=HabitTracker.db";

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
                        );";
                tableCmd.ExecuteNonQuery();
                
            }
            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (!closeApp)
            {
                Console.WriteLine("---Main Menu---");
                Console.WriteLine("0. Close app");
                Console.WriteLine("1. View All Records");
                Console.WriteLine("2. Add New Record");
                Console.WriteLine("3. Delete Record");
                Console.WriteLine("4. Edit Record");
                Console.WriteLine("5. Generate random values in table ");
                Console.WriteLine("6. Clear All Records");
                Console.WriteLine("-----------------------\n");
                
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("Bye bye!");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        ViewAllRecords();
                        break;
                    case "2":
                        AddNewRecord();
                        break;
                    case "3":
                        DeleteRecord();
                        break;
                    case "4":
                        EditRecord();
                        break;
                    case "5":
                        SeedRandomData();
                        break;
                    case "6":
                        ClearAllRecords();
                        break;
                    default:
                        Console.WriteLine("Invalid Command. Try again!");
                        break;
                }
            }
        }

        private static void ViewAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water;";

                List<DrinkingWater> tableData = new();
                
                SqliteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                ID = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")), 
                                Quantity = reader.GetInt32(2)
                            }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No records found!");
                    Console.WriteLine("Press any key to return to the main menu.");
                    Console.ReadKey();
                    return;
                }
                
                
                Console.WriteLine("-----------------------");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.ID} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("-----------------------\n");
            }
        }

        private static void AddNewRecord()
        {
            Console.Clear();
            string date = GetDateInput();
            
            int quantity = GetNumberInput("\n\nEnter the unit of measurement of your choice (decimals are not allowed)\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "INSERT INTO drinking_water (Date, Quantity) VALUES (@date, @quantity)";
                tableCmd.Parameters.AddWithValue("@date", date);
                tableCmd.Parameters.AddWithValue("@quantity", quantity);
                tableCmd.ExecuteNonQuery();
                
            }
        }

        private static void DeleteRecord()
        {
            Console.Clear();
            ViewAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to delete or type 0 to Main Menu\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "DELETE FROM drinking_water WHERE ID = @recordId";
                tableCmd.Parameters.AddWithValue("@recordId", recordId);
                
                
                int rowCount = tableCmd.ExecuteNonQuery();

                while (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist!\n\n");
                    recordId = GetNumberInput("Please type a valid ID or type 0 to return to main menu:");
                    if (recordId == 0) return;

                    tableCmd.CommandText = "DELETE FROM drinking_water WHERE ID = @recordId";
                    tableCmd.Parameters.Clear();
                    tableCmd.Parameters.AddWithValue("@recordId", recordId);

                    rowCount = tableCmd.ExecuteNonQuery();
                }
            }
            
            GetUserInput();
        }

        private static void EditRecord()
        {
            Console.Clear();    

            ViewAllRecords();
            
            var recordId = GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return to main manu.\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT EXISTS (SELECT 1 FROM drinking_water WHERE ID = @recordId)";
                checkCmd.Parameters.Clear();
                checkCmd.Parameters.AddWithValue("@recordId", recordId);
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                while (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist!\n\n");
                    recordId = GetNumberInput("Please type a valid ID or type 0 to return to main menu:");
                    if (recordId == 0) return;
                    checkCmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM drinking_water WHERE ID = @recordId)";
                    checkCmd.Parameters.AddWithValue("@recordId", recordId);
                    checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());
                }

                
                string date = GetDateInput();
                
                int quantity = GetNumberInput("\n\nEnter the unit of measurement of your choice (decimals are not allowed)\n\n");


                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "UPDATE drinking_water SET date = @date, quantity = @quantity WHERE ID = @recordId";
                tableCmd.Parameters.AddWithValue("@date", date);
                tableCmd.Parameters.AddWithValue("@quantity", quantity);
                tableCmd.Parameters.AddWithValue("@recordId", recordId);

                tableCmd.ExecuteNonQuery();
            }
        }
        
        private static void SeedRandomData()
        {
            Console.Clear();
            int numberOfValues = GetNumberInput("\n\nEnter the number of records you want to generate\n\n");
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var random = new Random();
                
                for (int i = 0; i < numberOfValues; i++)
                {
                    var randomDate = DateTime.Now.AddDays(-random.Next(numberOfValues)).ToString("dd-MM-yy");
                    var randomQuantity = random.Next(1, 11);
                    
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = "INSERT INTO drinking_water (Date, Quantity) VALUES(@date, @quantity)";
                    tableCmd.Parameters.AddWithValue("@date", randomDate);
                    tableCmd.Parameters.AddWithValue("@quantity", randomQuantity);
                    tableCmd.ExecuteNonQuery();
                }
            }
        }

        private static void ClearAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "DELETE FROM drinking_water;";
                tableCmd.ExecuteNonQuery();
            }
        }

        
        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main manu.\n\n");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main manu or try again:\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();
    
            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again.\n\n");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }

        public class DrinkingWater
        {
            public int ID { get; set; }
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
        }
        
        
    }
}
