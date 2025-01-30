using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace HabitTracker
{
    internal class Program
    {
        private static string connectionString = @"Data Source=HabitTracker.db";


        static void Main(string[] args)
        {
            InitializeTables();
            MenuSelection();
            
            
            //DateOnly date = GetDateInput("Enter date: ");
            //Console.WriteLine(date);
            //GetNumberInput("Enter number here: ");
        }

        private static void InitializeTables()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS drinkingWater
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                    )";

                // Add table for units of measurement
                // Link tables
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
        private static void PrintMainMenu()
        {
            Console.WriteLine("MAIN MENU");
            Console.WriteLine("---------------------------------------------------------\n");
            Console.WriteLine("Select an option below by entering the menu item number:");
            Console.WriteLine("\t0: Close Application.");
            Console.WriteLine("\t1: View All Records");
            Console.WriteLine("\t2: Insert Record");
            Console.WriteLine("\t3: Delete Record");
            Console.WriteLine("\t4: Update Record");
            Console.WriteLine();
        }
        private static void MenuSelection()
        {
            Console.Clear();
            bool closeApp = false;

            PrintMainMenu();

            while (closeApp == false)
            {
                string commandInput = GetUserInput("Enter menu selection: ");

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        InsertRecord();
                        break;
                    case "3":
                        DeleteRecord();
                        break;
                    case "4":
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("\nINVALID COMMAND! Please enter a number between 0 and 4.");
                        break;
                }

            }
        }
        private static void ReturnToMainMenu(string input)
        {
            if (input == "0") MenuSelection();
        }


        private static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        private static int GetNumberInput(string message)
        {
            string numberInput = string.Empty;
            int output;
            bool firstTime = true;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid number: ";
                numberInput = GetUserInput(message);
                firstTime = false;

            } while (!Int32.TryParse(numberInput, out output));

            return output;
        }
        private static DateOnly GetDateInput(string message)
        {
            string dateInput = string.Empty;
            DateOnly output;
            bool firstTime = true;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid date (yyyy-mm-dd): ";
                dateInput = GetUserInput(message);
                firstTime = false;

            } while (!DateOnly.TryParseExact(dateInput, "yyyy-MM-dd", out output));

            return output;
        }



        private static void UpdateRecord()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("Enter Id of the record you would like to update or type 0 to return to the main menu: ");
            ReturnToMainMenu(recordId.ToString());

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"select exists (Select 1 from drinkingWater where Id = {recordId}";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                    connection.Close();
                    UpdateRecord();
                }


                DateOnly date = GetDateInput("Please insert the date (Format yyyy-mm-dd): ");
                int quantitiy = GetNumberInput("Please insert number of units (no decimals allowed): ");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"update drinkingWater set Date = '{date.ToString("yyyy-MM-dd")}', quantity = '{quantitiy}' where Id = {recordId}";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static void DeleteRecord()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("Enter the Id of the record you want to delete or type 0 to return to the Main Menu: ");
            ReturnToMainMenu(recordId.ToString());

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"delete from drinkingWater where id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                    DeleteRecord();
                }
                Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");
            }
        }

        private static void InsertRecord()
        {
            string date = GetUserInput("Please insert the date (Format yyyy-mm-dd). Type 0 to return to main menu.");
            ReturnToMainMenu(date);


            int quantity = GetNumberInput("Please enter number of units (no decimals allowed): ");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"insert into drinkingWater(date, quantity) values('{date}', '{quantity}')";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }


        private static void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "select * from drinkingWater";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2),
                            }
                         );
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                Console.WriteLine("--------------------------------------------------------\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy")} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("--------------------------------------------------------\n");

            }
        }

    }

    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
