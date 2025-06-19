using HabitTrackerLibrary;
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
            db.InitializeTables();
            ConsoleUI.MainMenu(db);
        }
    }
}