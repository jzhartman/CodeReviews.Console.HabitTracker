using HabitTrackerLibrary;
using HabitTrackerLibrary.DataAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace HabitTracker
{
    internal class Program
    {
        private static string connectionString = @"Data Source=HabitTracker.db";
        private static SqliteDataAccess db = new SqliteDataAccess(connectionString);

        static void Main(string[] args)
        {
            DBInitializationData.InitializeTables(db);
            //db.InitializeTables();
            ConsoleUI.MainMenu(db);
        }
    }
}