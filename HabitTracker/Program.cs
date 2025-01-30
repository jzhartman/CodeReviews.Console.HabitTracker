using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace HabitTracker
{
    internal class Program
    {
        private static string connectionString = @"Data Source=HabitTracker.db";
        private static DataAccess db = new DataAccess(connectionString);


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
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.Clear();
                PrintMainMenu();
                string commandInput = GetUserInput("Enter menu selection: ");

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        break;
                    case "1":
                        //GetAllRecords();
                        PressAnyKeyToContinue();
                        break;
                    case "2":
                        InsertRecord();
                        break;
                    case "3":
                        //DeleteRecord();
                        break;
                    case "4":
                        //UpdateRecord();
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

        private static void PressAnyKeyToContinue()
        {
            Console.Write("Press any key to continue...");
            Console.ReadKey();
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
            bool invalidDate = true;
            bool firstTime = true;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid date (yyyy-mm-dd): ";
                dateInput = GetUserInput(message);
                ReturnToMainMenu(dateInput);
                firstTime = false;

                if(dateInput == "")
                {
                    invalidDate = false;
                    output = DateOnly.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd"));
                }
                else
                {
                    invalidDate = !DateOnly.TryParseExact(dateInput, "yyyy-MM-dd", out output);
                }

            } while (invalidDate);

            return output;
        }

        private static void InsertRecord()
        {
            DateOnly date = GetDateInput("Please enter the date (Format yyyy-mm-dd).\nType 0 to return to main menu.\nLeave blank for today's date.\nEnter selection: ");

            int quantity = GetNumberInput("Please enter number of units (no decimals allowed): ");

            db.Insert(date, quantity);
        }
    }

    public class HabitModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
