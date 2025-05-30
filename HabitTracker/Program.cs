using HabitTrackerLibrary;
using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace HabitTracker
{

    // Things seem decent... Need to continue from here and get it setup to work the way I planned previously
    // May need to pause and put more thought into the overall DB structure
    //      > How do Habits interact with Habit Records
    //      > Deleting a Habit deletes all records?
    internal class Program
    {
        private static string connectionString = @"Data Source=HabitTracker.db";
        private static SqliteDataAccess db = new SqliteDataAccess(connectionString);


        static void Main(string[] args)
        {
            db.InitializeTables();
            ConsoleUI.MenuSelection();


            ConsoleUI.PrintHabitsList(db);

            //db.InsertRecord("drinkingWater", "2025-03-07", 25);
            //db.UpdateRecord("drinkingWater", 8, "2024-03-07", 25);
            //db.UpdateRecord("drinkingWater", 9, "2025-03-07", 2);
            //db.DeleteRecord("drinkingWater", 8);


            // All CRUD operations works as written so far...
            //var habits = db.GetAllRecords("drinkingWater");
            //var habitsSorted = habits.OrderBy(o => o.Date).ToList();

            //foreach (var record in habitsSorted.Select((value, i) => (value, i)))
            //{
            //    Console.WriteLine($"{record.i + 1}: Drank {record.value.Quantity} units on {record.value.Date.ToString("yyyy-MM-dd")}");
            //}

        }



        private static void PrintAllRecords(string tableName)
        {

        }


        
    }
}