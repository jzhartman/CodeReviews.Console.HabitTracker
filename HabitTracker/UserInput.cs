using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    internal static class UserInput
    {
        internal static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        internal static int GetNumberInput(string message, int min, int max, bool allowBlanks = false)
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

                if (allowBlanks == true && numberInput == "")
                {
                    output = Int32.MinValue;
                    validNumber = true;
                }

            } while (validNumber == false);

            return output;
        }
        internal static DateOnly GetDateInput(string message, string blankBehavior)
        {
            string dateInput;
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
                    if (blankBehavior == "today")
                    {
                        output = DateOnly.FromDateTime(DateTime.Now);
                        validDate = true;
                    }
                    else if (blankBehavior == "original")
                    {
                        output = DateOnly.MinValue;
                        validDate = true;
                    }
                    else
                    {
                        output = DateOnly.MinValue;
                    }
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
