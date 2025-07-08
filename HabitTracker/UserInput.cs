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
        private static readonly string dateFormat = "yyyy-MM-dd";

        internal static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        internal static double GetNumberInput(string message, double min, double max, bool allowBlanks = false)
        {
            string numberInput = string.Empty;
            double output;
            bool firstTime = true;
            bool validNumber = false;

            do
            {
                message = (firstTime) ? message : "ERROR: Please enter a valid number! ";
                numberInput = GetUserInput(message);
                firstTime = false;

                validNumber = Double.TryParse(numberInput, out output);

                validNumber = (output < min || output > max) ? false : true;

                if (allowBlanks == true && numberInput.Trim() == "")
                {
                    output = Int32.MinValue;
                    validNumber = true;
                }

            } while (validNumber == false);

            return output;
        }
        internal static DateOnly GetDateInput(string message, string blankBehavior = "unused")
        {
            string dateInput;
            DateOnly output;
            bool firstTime = true;
            bool validDate = false;

            do
            {
                message = (firstTime) ? message : $"ERROR: Please enter a valid date ({dateFormat}): ";
                dateInput = GetUserInput(message);

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
                    validDate = DateOnly.TryParseExact(dateInput, dateFormat, out output);
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
        internal static bool GetUserConfirmation(string action)
        {
            bool confirmationResult = false;
            bool responseValid = true;

            do
            {
                string response = UserInput.GetUserInput($"Confirm {action}: Press \"Y\" for yes or \"N\" for no: ");

                if (response.ToLower().Trim() == "y")
                {
                    confirmationResult = true;
                    responseValid = true;
                }
                else if (response.ToLower().Trim() == "n")
                {
                    confirmationResult = false;
                    responseValid = true;
                }
                else
                {
                    Console.Write("INVALID RESPONSE!");
                    responseValid = false;
                }
            } while (responseValid == false);

            return confirmationResult;
        }
    }
}
