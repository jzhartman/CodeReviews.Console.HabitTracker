using HabitTrackerLibrary;
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


            //ConsoleUI.PrintHabitsList(db);

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

            //db.InitializeTables();
            //MenuSelection();
        }



        private static void PrintAllRecords(string tableName)
        {

        }


        
    }
}