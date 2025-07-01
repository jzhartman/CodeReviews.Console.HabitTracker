using HabitTrackerLibrary.DataAccess;
using HabitTrackerLibrary.Models;
using System.Diagnostics;

namespace HabitTracker
{
    /*
     *  Left off refactoring around line 334
     *  
     *  
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

        public ConsoleUI(SqlData sqlData)
        {
           this.sqlData = sqlData;
        }

        internal void PrintHabitsList()
        {
            var habits = sqlData.GetAllHabits();

            Console.WriteLine("Habits List");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var habit in habits.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{habit.i + 1}:\t{habit.value.HabitName}");
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
        }
        internal void PrintUnitsList()
        {
            var units = sqlData.GetAllUnits();

            Console.WriteLine("Units List");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            foreach (var unit in units.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"\t{unit.i + 1}: {unit.value.UnitName}");
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
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
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();
        }
        private void PrintRecordsListForHabit(HabitModel habit)
        {
            bool returnToMainMenu = false;

            while (returnToMainMenu == false)
            {
                var sortedRecords = SortRecordsList(habit);

                Console.Clear();
                PrintStatusBar($"View Records For Habit: {habit.HabitName}");
                PrintRecordsList(sortedRecords, habit);

                returnToMainMenu = MenuHandler_SelectHabit(habit, sortedRecords);
            }
        }
        internal void PrintStatusBar(string message)
        {
            Console.Clear();
            Console.WriteLine("Habit Tracker Application: Version 0.1 BETA");
            Console.WriteLine("---------------------------------------------------------");
            //Console.WriteLine(message);
            //Console.WriteLine();
            Console.WriteLine();
        }
        internal void PrintGoodbyeMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Goodbye!");
            Console.WriteLine();
        }
        internal void PrintMainMenu()
        {
            Menu mainMenu = new Menu
            (
                "Main Menu",
                "Select an option below by entering the menu item number:",
                new List<string>()
                {
                    "Select Habit",
                    "Add New Habit",
                    "Delete Habit",
                    "Exit Application"
                }
            );

            mainMenu.PrintMenu();
        }
        internal void PrintSelectHabitMenu()
        {
            Menu viewHabitMenu = new Menu
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

            viewHabitMenu.PrintMenu();
        }



        internal void MenuHandler_MainMenu()
        {
            bool closeApp = false;

            while (closeApp == false)
            {
                PrintStatusBar("Main Menu");
                PrintHabitsList();
                PrintMainMenu();

                int userSelection = UserInput.GetNumberInput("Enter menu selection: ", 1, 4);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        ProcessManager_SelectHabit();
                        break;
                    case "2":
                        ProcessManager_CreateHabit();
                        break;
                    case "3":
                        ProcessManager_DeleteHabit();
                        break;
                    case "4":
                        PrintGoodbyeMessage();
                        closeApp = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("ERROR - Invalid selection detected");
                        break;
                }
            }
        }
        private bool MenuHandler_SelectHabit(HabitModel habit, List<RecordModel> sortedRecords)
        {
            bool returnToMainMenu = false;
            bool validInput = true;

            PrintSelectHabitMenu();

            do
            {
                int userSelection = UserInput.GetNumberInput("Enter menu selection: ", 1, 5);
                string commandInput = userSelection.ToString();

                switch (commandInput)
                {
                    case "1":
                        AddRecord(habit);
                        break;
                    case "2":
                        ProcessManager_DeleteRecord(sortedRecords, habit);
                        break;
                    case "3":
                        UpdateRecord(sortedRecords, habit);
                        break;
                    case "4":
                        returnToMainMenu = true;
                        break;
                    default:
                        validInput = false;
                        Console.WriteLine();
                        Console.WriteLine("ERROR - Invalid selection detected");
                        break;
                }
            } while (validInput == false);

            return returnToMainMenu;
        }




        private void ProcessManager_SelectHabit()
        {
            PrintStatusBar("Habit Selection");
            PrintHabitsList();
            var habit = GetHabitFromList("view");
            PrintRecordsListForHabit(habit);
        }
        private void ProcessManager_CreateHabit()
        {
            PrintStatusBar("");
            PrintHabitsList();

            var habitName = GetNewHabitName();

            PrintStatusBar("");
            var unitName = GetUnitNameFromList();

            ConfirmNewHabitData(habitName, unitName);

            UserInput.PressAnyKeyToContinue();
        }
        private void ProcessManager_DeleteHabit()
        {
            PrintHabitsList();

            var habit = GetHabitFromList("delete");

            var recordCount = SortRecordsList(habit).Count();

            Console.WriteLine();
            Console.WriteLine($"WARNING: Deleting {habit.HabitName} will also delete the {recordCount} records associated with it. This cannot be undone.");
            Console.WriteLine();

            ConfirmHabitDelete(habit);


            Console.WriteLine();

            UserInput.PressAnyKeyToContinue();
        }
        private void ProcessManager_DeleteRecord(List<RecordModel> sortedRecords, HabitModel habit)
        {
            var record = GetRecordFromList(sortedRecords, "delete", habit);
            Console.WriteLine();
            Console.WriteLine($"Preparing to delete record for {record.Quantity} {habit.UnitName} of {habit.HabitName} on {record.Date.ToString("yyyy-MM-dd")}");

            ConfirmRecordDelete(record);
        }





        private void ConfirmHabitDelete(HabitModel habit)
        {
            bool habitDeleted = UserInput.GetUserConfirmation("delete");
            string message = $"User cancelled the deletion of habit {habit.HabitName}.";

            if (habitDeleted)
            {
                sqlData.DeleteAllRecordsForAHabit(habit.HabitId);
                sqlData.DeleteHabit(habit.HabitId);

                message = $"Habit {habit.HabitName} was successfully deleted!";
            }

            Console.WriteLine(message);
        }

        private void ConfirmRecordDelete(RecordModel record)
        {
            bool recordDeleted = UserInput.GetUserConfirmation("delete");
            string message = "Record delete cancelled!";


            if (recordDeleted)
            {
                message = "Record successfuly deleted!";
                sqlData.DeleteRecord("Records", record.Id);
            }

            Console.WriteLine(message);
        }




        internal List<RecordModel> SortRecordsList(HabitModel habit)
        {
            var records = sqlData.GetAllRecords(habit.HabitId);

            List<RecordModel> sortedRecords = records.OrderBy(o => o.Date).ToList();

            return sortedRecords;
        }


        private HabitModel GetHabitFromList(string action)
        {
            var habitList = sqlData.GetAllHabits();

            int userSelection = UserInput.GetNumberInput($"Enter ID of the Habit you wish to {action}: ", 1, habitList.Count());
            var habit = habitList[userSelection - 1];

            return habit;
        }
        private RecordModel GetRecordFromList(List<RecordModel> sortedRecords, string action, HabitModel habit)
        {
            int userSelection = UserInput.GetNumberInput($"Enter ID of the record you wish to {action}: ", 1, sortedRecords.Count());
            var record = sortedRecords[userSelection - 1];

            return record;
        }



        private void UpdateRecord(List<RecordModel> sortedRecords, HabitModel habit)
        {
            string confirmationCheck = string.Empty; ;
            bool dateChanged = true;

            var record = GetRecordFromList(sortedRecords, "change", habit);
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

        private (DateOnly date, int quantity) GetRecordData(HabitModel habit)
        {
            var date = UserInput.GetDateInput($"Enter the date when {habit.HabitName} occurred using YYYY-MM-DD format (leave blank to add today's date): ", "today");

            Debug.WriteLine($"Date Input: {date.ToString("yyyy-MM-dd")}");

            var quantity = UserInput.GetNumberInput($"Enter the quantity to record (Unit = {habit.UnitName}): ", 1, Int32.MaxValue);

            return (date, quantity);
        }

        private void AddRecord(HabitModel habit)
        {
            Console.WriteLine($"Tracking record for {habit.HabitName}");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            (DateOnly date, int quantity) = GetRecordData(habit);

            Console.WriteLine();
            Console.WriteLine($"Adding record of {quantity} {habit.UnitName} of {habit.HabitName} on {date.ToString("yyyy-MM-dd")}!");

            sqlData.InsertRecord(habit.HabitId, date.ToString("yyyy-MM-dd"), quantity);
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

        private string GetUnitNameFromList()
        {
            PrintUnitsList();

            var unitName = string.Empty;
            var unitList = sqlData.GetAllUnits();

            int userSelection = UserInput.GetNumberInput($"Enter ID of the unit you wish to use (or enter blank to create a new unit): ", 1, unitList.Count(), true);

            
            if (userSelection == Int32.MinValue)
            {
                unitName = GetNewUnitName();
            }
            else if (userSelection <= unitList.Count())
            {
                unitName = unitList[userSelection - 1].UnitName;
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
