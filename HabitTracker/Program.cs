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
            var sqlData = new SqlData(db, connectionString);
            var consoleUI = new ConsoleUI(sqlData);

            DBInitializationData.InitializeTables(db, sqlData);

            consoleUI.MainMenu();
        }
    }
}