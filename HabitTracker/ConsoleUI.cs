using HabitTrackerLibrary.DataAccess;
using HabitTrackerLibrary.Models;
using System.Diagnostics;

namespace HabitTracker
{
    /*
     *  ReadMe file
     *  Parameterized queries
     *  Add report functionality
     *  
     *  
     *  TODO: Allow user to escape entries during input operations
     *  TODO: Allow user to update habit (change name, or change unit)
     *  
     */

    public class ConsoleUI
    {
        private readonly SqlData sqlData;

        private Menu mainMenu = new Menu
            (
                "Main Menu",
                "Select an option below by entering the menu item number:",
                new List<string>()
                {
                    "Track Habit",
                    "View Habit",
                    "Add New Habit",
                    "Delete Habit",
                    "Exit Application"
                }

            );

        private Menu viewHabitMenu = new Menu
            (
                "View Habit",
                "Select an option below by entering the menu item number:",
                new List<string>()
                {
                    "Add Record",
                    "Delete Record",
                    "Change Record",
                    "Return to Main Menu"
                }
            );

        public ConsoleUI(SqlData sqlData)
        {
           this.sqlData = sqlData;
        }

        internal void PrintHabitsList(bool printMainMenu)
        {
            Console.Clear();
            WelcomeMessage();

            var habits = sqlData.GetAllHabits();

            Console.WriteLine("Habits List");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var habit in habits.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{habit.i + 1}:\t{habit.value.HabitName}");
            }

            Console.WriteLine();
            if (printMainMenu) mainMenu.PrintMenu();
            Console.WriteLine();
        }

        internal void PrintUnitsList(string habitName)
        {
            var units = sqlData.GetAllUnits();

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Select the unit to use when tracking {habitName}:");
            Console.WriteLine();
            Console.WriteLine("Units List");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var unit in units.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{unit.i + 1}: {unit.value.UnitName}");
            }

            Console.WriteLine($"\t{units.Count + 1}: <Enter New Unit>");

            Console.WriteLine();
        }

        internal List<RecordModel> GetSortedRecordsList(HabitModel habit)
        {
            var records = sqlData.GetAllRecords(habit.HabitId);

                List<RecordModel> sortedRecords = records.OrderBy(o => o.Date).ToList();

            return sortedRecords;
        }
        internal void PrintRecordsList(List<RecordModel> sortedRecords, HabitModel habit)
        {
            Console.WriteLine($"Records List for {habit.HabitName}");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var record in sortedRecords.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{record.i + 1}: {record.value.Date.ToString("yyyy-MM-dd")}\t{record.value.Quantity}\t{habit.UnitName}");
            }

            Console.WriteLine();
        }

        internal void WelcomeMessage()
        {
            Console.WriteLine("Habit Tracker Application: Version 0.1 BETA");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }

        internal void MainMenu()
        {
            bool closeApp = false;

            while (closeApp == false)
            {
                PrintHabitsList(true);
                int userSelection = UserInput.GetNumberInput("Enter menu selection: ", 1, 5);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        TrackHabit();
                        break;
                    case "2":
                        ViewHabitRecords();
                        break;
                    case "3":
                        CreateHabit();
                        break;
                    case "4":
                        DeleteHabit();
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

        private void DeleteHabit()
        {
            PrintHabitsList(false);

            var habit = SelectHabitFromList("delete");

            var recordCount = GetSortedRecordsList(habit).Count();

            Console.WriteLine();
            Console.WriteLine($"WARNING: Deleting {habit.HabitName} will also delete the {recordCount} records associated with it. This cannot be undone.");
            Console.WriteLine();

            var habitDeleted = ConfirmHabitDelete(habit);

            var message = (habitDeleted) ? $"Habit {habit.HabitName} was successfully deleted!" : $"User cancelled the deletion of habit {habit.HabitName}.";

            Console.WriteLine(message);
            Console.WriteLine();

            UserInput.PressAnyKeyToContinue();
        }

        private bool ConfirmHabitDelete(HabitModel habit)
        {
            bool habitDeleted = false;
            bool responseValid = false;

            while (responseValid == false)
            {
                string response = UserInput.GetUserInput("Confirm delete: Press \"Y\" for yes or \"N\" for no: ");

                if (response.ToLower() == "y")
                {
                    sqlData.DeleteAllRecordsForAHabit(habit.HabitId);
                    sqlData.DeleteHabit(habit.HabitId);
                    responseValid = true;
                    habitDeleted = true;
                }
                else if (response.ToLower() == "n")
                {
                    responseValid = true;
                }
                else
                {
                    Console.Write("INVALID RESPONSE! ");
                    responseValid = false;
                }
            }

            return habitDeleted;
        }

        private HabitModel SelectHabitFromList(string action)
        {
            var habitList = sqlData.GetAllHabits();

            int userSelection = UserInput.GetNumberInput($"Enter ID of the Habit you wish to {action}: ", 1, habitList.Count());
            var habit = habitList[userSelection - 1];

            return habit;
        }

        private void ViewHabitRecords()
        {
            PrintHabitsList(false);
            var habit = SelectHabitFromList("view");
            ViewHabitRecords_PrintRecordsList(habit);
        }

        private void ViewHabitRecords_PrintRecordsList(HabitModel habit)
        {
            bool returnToMainMenu = false;

            while (returnToMainMenu == false)
            {
                var sortedRecords = GetSortedRecordsList(habit);

                Console.Clear();
                WelcomeMessage();
                PrintRecordsList(sortedRecords, habit);

                returnToMainMenu = ViewHabitRecords_HandleUserSelection(habit, sortedRecords);
            }
        }

        private bool ViewHabitRecords_HandleUserSelection(HabitModel habit, List<RecordModel> sortedRecords)
        {
            bool returnToMainMenu = false;
            bool validInput = false;

            viewHabitMenu.PrintMenu();
            
            while (validInput == false)
            {
                int userSelection = UserInput.GetNumberInput("Enter menu selection: ", 1, 5);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        CreateRecord(habit);
                        validInput = true;
                        break;
                    case "2":
                        DeleteHabitRecord(sortedRecords, habit);
                        validInput = true;
                        break;
                    case "3":
                        UpdateHabitRecord(sortedRecords, habit);
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
                UserInput.PressAnyKeyToContinue();
            }

            return returnToMainMenu;
        }

        private void DeleteHabitRecord(List<RecordModel> sortedRecords, HabitModel habit)
        {
            var record = SelectRecordFromList(sortedRecords, "delete", habit);
            Console.WriteLine();
            Console.WriteLine($"Preparing to delete record for {record.Quantity} {habit.UnitName} of {habit.HabitName} on {record.Date.ToString("yyyy-MM-dd")}");


            bool responseValid = false;

            while (responseValid == false)
            {
                string response = UserInput.GetUserInput("Confirm delete: Press \"Y\" for yes or \"N\" for no: ");

                if (response.ToLower() == "y")
                {
                    Console.WriteLine("Record successfuly deleted!");
                    sqlData.DeleteRecord("Records", record.Id);
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

        private RecordModel SelectRecordFromList(List<RecordModel> sortedRecords, string action, HabitModel habit)
        {
            int userSelection = UserInput.GetNumberInput($"Enter ID of the record you wish to {action}: ", 1, sortedRecords.Count());
            var record = sortedRecords[userSelection - 1];

            return record;
        }

        private void UpdateHabitRecord(List<RecordModel> sortedRecords, HabitModel habit)
        {
            string confirmationCheck = string.Empty; ;
            bool dateChanged = true;

            var record = SelectRecordFromList(sortedRecords, "change", habit);
            Console.WriteLine();
            Console.WriteLine($"Changing record for {record.Quantity} {habit.UnitName} of {habit.HabitName} on {record.Date.ToString("yyyy-MM-dd")}");


            var newDate = UserInput.GetDateInput($"Enter the new date (or press enter to keep original date): ", "original");

            if (newDate == DateOnly.MinValue)
            {
                newDate = DateOnly.FromDateTime(record.Date);
                dateChanged = false;
            }
            else
            {
                confirmationCheck = $"Changed date from {record.Date.ToString("yyyy-MM-dd")} to {newDate.ToString("yyyy-MM-dd")}";
            }

            var newQuantity = UserInput.GetNumberInput($"Enter the new quantity for the record (or press enter to keep original quantity): ", 1, Int32.MaxValue, true);

            if (newQuantity == Int32.MinValue)
            {
                newQuantity = record.Quantity;

                if (dateChanged)
                {
                    confirmationCheck += ".";
                }
            }
            else
            {
                if (dateChanged)
                {
                    confirmationCheck += $" and changed quantity from {record.Quantity} to {newQuantity}.";
                }
                else
                {
                    confirmationCheck = $"Changed quantity from {record.Quantity} to {newQuantity}.";
                }
            }

                bool responseValid = false;

            if (DateOnly.FromDateTime(record.Date) == newDate && record.Quantity == newQuantity) confirmationCheck = "No changes made to record.";

            Console.WriteLine(confirmationCheck);


            while (responseValid == false)
            {
                string response = UserInput.GetUserInput("Confirm update: Press \"Y\" for yes or \"N\" for no: ");

                if (response.ToLower() == "y")
                {
                    Console.WriteLine("Record successfuly updated!");
                    sqlData.UpdateRecord("Records", record.Id, newDate.ToString("yyyy-MM-dd"), newQuantity);
                    responseValid = true;
                }
                else if (response.ToLower() == "n")
                {
                    Console.WriteLine("Update cancelled!");
                    responseValid = true;
                }
                else
                {
                    Console.Write("INVALID RESPONSE! ");
                    responseValid = false;
                } 
            }
        }

        private void TrackHabit()
        {
            PrintHabitsList(false);

            var habit = SelectHabitFromList("track");

            Console.Clear();
            WelcomeMessage();

            CreateRecord(habit);

            UserInput.PressAnyKeyToContinue();
        }

        private (DateOnly date, int quantity) GetRecordData(HabitModel habit)
        {
            var date = UserInput.GetDateInput($"Enter the date when {habit.HabitName} occurred using YYYY-MM-DD format (leave blank to add today's date): ", "today");

            Debug.WriteLine($"Date Input: {date.ToString("yyyy-MM-dd")}");

            var quantity = UserInput.GetNumberInput($"Enter the quantity to record (Unit = {habit.UnitName}): ", 1, Int32.MaxValue);

            return (date, quantity);
        }

        private void CreateRecord(HabitModel habit)
        {
            Console.WriteLine($"Tracking record for {habit.HabitName}");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            (DateOnly date, int quantity) = GetRecordData(habit);

            Console.WriteLine();
            Console.WriteLine($"Adding record of {quantity} {habit.UnitName} of {habit.HabitName} on {date.ToString("yyyy-MM-dd")}!");

            sqlData.InsertRecord(habit.HabitId, date.ToString("yyyy-MM-dd"), quantity);
        }

        private void CreateHabit()
        {
            PrintHabitsList(false);

            var habitName = GetNewHabitName();

            var unitName = GetUnitForNewHabit(habitName);

            ConfirmNewHabitData(habitName, unitName);

            UserInput.PressAnyKeyToContinue();
        }

        private void ConfirmNewHabitData(string habitName, string unitName)
        {
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Preparing to add new habit, {habitName}, measured with units, {unitName}.");
            bool responseValid = false;

            while (responseValid == false)
            {
                string response =   UserInput.GetUserInput("To confirm enter \"Y\" for yes or \"N\" for no: ");
                Console.WriteLine();

                if (response.ToLower() == "y")
                {
                    Console.WriteLine("New habit successfuly added!");
                    sqlData.InsertUnit(unitName);
                    sqlData.InsertHabit(habitName, unitName);
                    responseValid = true;
                }
                else if (response.ToLower() == "n")
                {
                    Console.WriteLine("New habit entry cancelled!");
                    responseValid = true;
                }
                else
                {
                    Console.Write("INVALID RESPONSE! ");
                    responseValid = false;
                }
            }

        }

        private string GetUnitForNewHabit(string habitName)
        {
            PrintUnitsList(habitName);

            var unitName = string.Empty;
            var unitList = sqlData.GetAllUnits();

            int userSelection = UserInput.GetNumberInput($"Enter ID of the unit you wish to use: ", 1, unitList.Count() + 1);

            if (userSelection <= unitList.Count())
            {
                unitName = unitList[userSelection - 1].UnitName;
            }
            else if(userSelection == unitList.Count() + 1)
            {
                unitName = GetNewUnitName();
            }

            return unitName;
        }
        private string GetNewUnitName()
        {
            var unitName = string.Empty;
            var status = string.Empty;
            bool unitExists = true;

            while (unitExists == true)
            {
                unitName = GetNameFromUser("unit");
                unitExists = sqlData.CheckIfUnitExists(unitName);

                if (unitExists == true)
                {
                    Console.WriteLine("ERROR! Unit already exists");
                }
            }

            return unitName;
        }

        private string GetNewHabitName()
        {
            var habitName = string.Empty;
            var status = string.Empty;
            bool habitExists = true;

            while (habitExists == true)
            {
                habitName = GetNameFromUser("habit");
                habitExists = sqlData.CheckIfHabitExists(habitName);

                if (habitExists == true)
                {
                    Console.WriteLine("ERROR! Habit already exists");
                }
            }

            return habitName;
        }
        private string GetNameFromUser(string itemType)
        {
            string name = string.Empty;
            bool validName = false;

            while (validName == false)
            {
                name = UserInput.GetUserInput($"Enter the name of the new {itemType}: ");

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine($"INVALID {itemType.ToUpper()} NAME!");
                }
                else
                {
                    validName = true;
                }
            }

            return name;
        }
    }
}
