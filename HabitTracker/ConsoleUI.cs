using HabitTrackerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    /*
     *  Need to write out the overall application flow for menu items
     *      Where do we start?
     *      How to select and view habits
     *      how to add or delete habits?
     */

    internal static class ConsoleUI
    {
        internal static void PrintHabitsList(SqliteDataAccess db)
        {
            var habits = db.GetAllHabits();

            foreach (var habit in habits.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"{habit.i + 1}: {habit.value.Name}");
            }
        }

        internal static void PrintMainMenu()
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

        internal static void MenuSelection()
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
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    default:
                        Console.WriteLine("\nINVALID COMMAND! Please enter a number between 0 and 4.");
                        break;
                }

            }
        }




        internal static void ReturnToMainMenu(string input)
        {
            if (input == "0") MenuSelection();
        }
        internal static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        internal static int GetNumberInput(string message)
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
        internal static DateOnly GetDateInput(string message)
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
    }
}
