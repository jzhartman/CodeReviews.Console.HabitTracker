using HabitTrackerLibrary;
using HabitTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    /*
     *  Currently working on: TrackHabit method
     *      - Inputs working so far
     *      - HabitModel stores UnitsID and not actual unit data -- need to figure out how to map this with ADO
     *      - Insert Record works!
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
                Console.WriteLine($"\t{habit.i + 1}: {habit.value.Name}");
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
            Console.WriteLine("\t2: View  Habit");
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

        private static void TrackHabit(SqliteDataAccess db)
        {
            Console.Clear();
            WelcomeMessage();
            PrintHabitsList(db);
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("Track Habit");
            Console.WriteLine();


            var habitList = db.GetAllHabits();

            int userSelection = GetNumberInput("Enter ID of the Habit you wish to track: ", 1, habitList.Count());
            var trackedHabit = habitList[userSelection - 1];

            Console.WriteLine();
            Console.WriteLine($"Selected habit: {trackedHabit.Name}");


            var date = GetDateInput($"Enter the date when {trackedHabit.Name} occurred using YYYY-MM-DD format (leave blanke to add today's date): ");

            Debug.WriteLine($"Date Input: {date.ToString("yyyy-MM-dd")}");

            var quantity = GetNumberInput($"Enter the quantity to record (Unit = {trackedHabit.Units}): ", 1, Int32.MaxValue);

            Console.WriteLine();
            Console.WriteLine($"Adding record of {quantity} units of {trackedHabit.Name} on {date.ToString("yyyy-MM-dd")}!");

            db.InsertRecord("Records", trackedHabit.Id, date.ToString("yyyy-MM-dd"), quantity);

            PressAnyKeyToContinue();
        }


        //internal static void ReturnToMainMenu(string input)
        //{
        //    if (input == "0") MainMenu();
        //}
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
