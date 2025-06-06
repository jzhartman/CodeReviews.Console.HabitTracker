using HabitTrackerLibrary;
using HabitTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HabitTracker
{
    /*
     * 
     *  Currently working on: ViewHabit method
     *      - (COMPLETE) Print all records for a selected habit
     *      - (COMPLETE) Add new record to list, then reprint
     *      - (COMPLETE) Delete a record from the list, then reprint
     *      - Update a record in the list, then reprint
     *  
     *  Needs
     *      - Flesh out DeleteRecord method
     *      - Flesh out UpdateRecord method
     *  
     *  Need to add a way to allow user to cancel input operation....
     */

    internal static class ConsoleUI
    {
        internal static void PrintHabitsList(SqliteDataAccess db)
        {
            var habits = db.GetAllHabits();

            Console.WriteLine("Habits List");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var habit in habits.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{habit.i + 1}: {habit.value.HabitName}");
            }

            Console.WriteLine();
        }

        internal static List<RecordModel> GetSortedRecordsList(SqliteDataAccess db, HabitModel habit)
        {
            var records = db.GetAllRecords(habit.HabitId);

            List<RecordModel> sortedRecords = records.OrderBy(o => o.Date).ToList();

            return sortedRecords;
        }
        internal static void PrintRecordsList(List<RecordModel> sortedRecords, HabitModel habit)
        {
            //var sortedRecords = GetSortedRecordsList(db, habit);

            Console.WriteLine($"Records List for {habit.HabitName}");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var record in sortedRecords.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{record.i + 1}: {record.value.Date.ToString("yyyy-MM-dd")}\t{record.value.Quantity}\t{habit.UnitsPlural}");
            }

            Console.WriteLine();
        }

        internal static void WelcomeMessage()
        {
            Console.WriteLine("Habit Tracker Application: Version 0.1 BETA");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }

        internal static void PrintMainMenu()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("Select an option below by entering the menu item number:");
            Console.WriteLine();
            Console.WriteLine("\t1: Track Habit");
            Console.WriteLine("\t2: View Habit");
            Console.WriteLine("\t3: Add New Habit");
            Console.WriteLine("\t4: Delete Habit");
            Console.WriteLine("\t5: Exit Application");
            Console.WriteLine();  
        }

        internal static void MainMenu(SqliteDataAccess db)
        {
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.Clear();
                WelcomeMessage();
                PrintHabitsList(db);
                PrintMainMenu();
                Console.WriteLine("---------------------------------------------------------");
                int userSelection = GetNumberInput("Enter menu selection: ", 1, 5);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        TrackHabit(db);
                        break;
                    case "2":
                        ViewHabitRecords(db);
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        Console.WriteLine();
                        Console.WriteLine("Goodbye!");
                        Console.WriteLine();
                        closeApp = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("ERROR - Invalid selection detected");
                        break;
                }

            }
        }

        private static HabitModel SelectHabitFromList(SqliteDataAccess db, string action)
        {
            var habitList = db.GetAllHabits();

            int userSelection = GetNumberInput($"Enter ID of the Habit you wish to {action}: ", 1, habitList.Count());
            var habit = habitList[userSelection - 1];

            return habit;
        }

        private static void ViewHabitRecords(SqliteDataAccess db)
        {
                
            ViewHabitRecords_PrintHabitList(db);
            var habit = SelectHabitFromList(db, "view");
            ViewHabitRecords_PrintRecordsList(db, habit);
         
        }

        private static void ViewHabitRecords_PrintHabitList(SqliteDataAccess db)
        {
            Console.Clear();
            WelcomeMessage();
            PrintHabitsList(db);
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
        }

        private static void ViewHabitRecords_PrintRecordsList(SqliteDataAccess db, HabitModel habit)
        {
            bool returnToMainMenu = false;

            while (returnToMainMenu == false)
            {
                var sortedRecords = GetSortedRecordsList(db, habit);

                Console.Clear();
                WelcomeMessage();
                PrintRecordsList(sortedRecords, habit);

                returnToMainMenu = ViewHabitRecords_HandleUserSelection(db, habit, sortedRecords);
            }
        }

        private static void ViewHabitRecords_PrintSubMenu()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("Select an option below by entering the menu item number:");
            Console.WriteLine();
            Console.WriteLine("\t1: Add Record");
            Console.WriteLine("\t2: Delete Record");
            Console.WriteLine("\t3: Change Record");
            Console.WriteLine("\t4: Return to Main Menu");
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------");
        }

        private static bool ViewHabitRecords_HandleUserSelection(SqliteDataAccess db, HabitModel habit, List<RecordModel> sortedRecords)
        {
            bool returnToMainMenu = false;
            bool validInput = false;

            ViewHabitRecords_PrintSubMenu();
            
            while (validInput == false)
            {
                int userSelection = GetNumberInput("Enter menu selection: ", 1, 5);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        CreateRecord(db, habit);
                        validInput = true;
                        break;
                    case "2":
                        DeleteHabitRecord(db, sortedRecords, habit);
                        validInput = true;
                        break;
                    case "3":
                        UpdateHabitRecord(db, habit);
                        validInput = true;
                        break;
                    case "4":
                        Console.WriteLine("Returning to main menu...");
                        validInput = true;
                        returnToMainMenu = true;
                        break;
                    default:
                        validInput = false;
                        Console.WriteLine();
                        Console.WriteLine("ERROR - Invalid selection detected");
                        break;
                }
                PressAnyKeyToContinue();
            }

            return returnToMainMenu;
        }

        private static void DeleteHabitRecord(SqliteDataAccess db, List<RecordModel> sortedRecords, HabitModel habit)
        {
            var record = SelectRecordFromList(db, sortedRecords, "delete", habit);
            Console.WriteLine();
            Console.WriteLine($"Preparing to delete record for {record.Quantity} {habit.UnitsPlural} of {habit.HabitName} on {record.Date}");


            bool responseValid = false;

            while (responseValid == false)
            {
                string response = GetUserInput("Confirm delete: Press \"Y\" for yes or \"N\" for no: ");

                if (response.ToLower() == "y")
                {
                    Console.WriteLine("Record successfuly deleted!");
                    db.DeleteRecord("Records", record.Id);
                    responseValid = true;
                }
                else if (response.ToLower() == "n")
                {
                    Console.WriteLine("Delete cancelled!");
                    responseValid = true;
                }
                else
                {
                    Console.Write("INVALID RESPONSE! ");
                    responseValid = false;
                } 
            }
        }

        private static RecordModel SelectRecordFromList(SqliteDataAccess db, List<RecordModel> sortedRecords, string action, HabitModel habit)
        {
            int userSelection = GetNumberInput($"Enter ID of the record you wish to {action}: ", 1, sortedRecords.Count());
            var record = sortedRecords[userSelection - 1];

            return record;
        }

        private static void UpdateHabitRecord(SqliteDataAccess db, HabitModel habit)
        {
            Console.WriteLine("Someday this will let you update...");
            PressAnyKeyToContinue();
            Console.WriteLine();

            //Print current date
            //Enter new date -- blank == no change
            //Print current quantity
            //Enter new quantity - blank == no change
            //Print new data (maybe in reference to old?)
            //UpdateRecord
        }

        private static void TrackHabit(SqliteDataAccess db)
        {
            Console.Clear();
            WelcomeMessage();
            PrintHabitsList(db);
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();


            var habit = SelectHabitFromList(db, "track");

            Console.Clear();
            WelcomeMessage();

            CreateRecord(db, habit);

            PressAnyKeyToContinue();
        }

        private static (DateOnly date, int quantity) GetRecordData(HabitModel habit)
        {
            var date = GetDateInput($"Enter the date when {habit.HabitName} occurred using YYYY-MM-DD format (leave blanke to add today's date): ");

            Debug.WriteLine($"Date Input: {date.ToString("yyyy-MM-dd")}");

            var quantity = GetNumberInput($"Enter the quantity to record (Unit = {habit.UnitsPlural}): ", 1, Int32.MaxValue);

            return (date, quantity);
        }

        private static void CreateRecord(SqliteDataAccess db, HabitModel habit)
        {
            Console.WriteLine($"Tracking record for {habit.HabitName}");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            (DateOnly date, int quantity) = GetRecordData(habit);

            Console.WriteLine();
            Console.WriteLine($"Adding record of {quantity} {habit.UnitsPlural} of {habit.HabitName} on {date.ToString("yyyy-MM-dd")}!");

            db.InsertRecord("Records", habit.HabitId, date.ToString("yyyy-MM-dd"), quantity);
        }



        internal static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        internal static int GetNumberInput(string message, int min, int max)
        {
            string numberInput = string.Empty;
            int output;
            bool firstTime = true;
            bool validNumber = false;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid number (GNI): ";
                numberInput = GetUserInput(message);
                Debug.WriteLine($"User entered: {numberInput}");
                firstTime = false;

                validNumber = Int32.TryParse(numberInput, out output);

                validNumber = (output < min || output > max) ? false : true;

            } while (validNumber == false);

            return output;
        }
        internal static DateOnly GetDateInput(string message)
        {
            string dateInput = string.Empty;
            DateOnly output;
            bool firstTime = true;
            bool validDate = false;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid date (yyyy-mm-dd): ";
                dateInput = GetUserInput(message);
                Debug.WriteLine($"User entered: {dateInput}");

                if (dateInput == "")
                {
                    output = DateOnly.FromDateTime(DateTime.Now);
                    validDate = true;
                }
                else
                {
                    validDate = DateOnly.TryParseExact(dateInput, "yyyy-MM-dd", out output);
                }

                    firstTime = false;

            } while (validDate == false);

            return output;
        }
        internal static void PressAnyKeyToContinue()
        {
            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
