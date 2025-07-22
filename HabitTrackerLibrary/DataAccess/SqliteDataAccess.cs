using Microsoft.Data.Sqlite;
using System.Globalization;
using HabitTrackerLibrary.Models;
using HabitTrackerLibrary.DataAccess;

namespace HabitTrackerLibrary
{
    public class SqliteDataAccess
    {
        public string connectionStringName { get; set; }

        public SqliteDataAccess(string connectionString)
        {
            this.connectionStringName = connectionString;
        }

        internal void Execute(string sql)
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = sql;

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}