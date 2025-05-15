using Microsoft.Data.Sqlite;
using System.Globalization;
using HabitTrackerLibrary.Models;

namespace HabitTrackerLibrary
{
    public class DataAccess
    {
        public string connectionStringName { get; set; }

        public DataAccess(string connectionString)
        {
            this.connectionStringName = connectionString;
        }

        public void InitializeTables() // She needs a lot of work.........
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS habits
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Date TEXT,
                        Quantity INTEGER,
                        Units TEXT
                    )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

        }

        private void Execute(string sql)
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

        public void InsertRecord(string tableName, string date, int quantity)
        {
            Execute($"insert into {tableName}(Date, Quantity) values('{date}', '{quantity}')");
        }

        public void UpdateRecord(string tableName, int recordId, string date, int quantity)
        {
            Execute($"update {tableName} set Date = '{date}', quantity = '{quantity}' where Id = {recordId}");
        }

        public void DeleteRecord(string tableName, int recordId)
        {
            Execute($"delete from {tableName} where id = '{recordId}'");
        }

        public void InsertHabit(string name, string units)
        {
            Execute($"insert into habits (Name, Units) values('{name}', '{units}')");
        }

        public void UpdateHabit(string name, string units, int recordId)
        {
            Execute($"update habits set Name = '{name}', Units = '{units}' where Id = {recordId}");
        }

        public void DeleteHabit(int recordId)
        {
            Execute($"delete from habits where id = '{recordId}'");
        }

        public List<HabitRecordModel> GetAllRecords(string tableName) // Probably need to create separate method for Habits table
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"select * from {tableName}";

                List<HabitRecordModel> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new HabitRecordModel
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "yyyy-MM-dd", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2),
                            }
                         );
                    }
                }

                connection.Close();

                return tableData;
            }
        }

        public List<HabitModel> GetAllHabits() // Probably need to create separate method for Habits table
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"select * from habits";

                List<HabitModel> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new HabitModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Units = reader.GetString(2),
                            }
                         );
                    }
                }

                connection.Close();

                return tableData;
            }
        }

        public bool RecordExists(string tableName, int recordId)
        {
            bool recordExists = false;

            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();

                checkCmd.CommandText = $"select exists (Select 1 from {tableName} where Id = {recordId}";


                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                return (checkQuery == 0) ? false : true;
            }
        }
    }
}


// Insert and delete can be same function -- just return the value from the tableCmd.ExecuteNonQuery() method


// To get list of table names, create a table called Habits 


/*
 * Overall Flow:
 *      User opens app
 *          Create the following:
 *              Create database if none exists
 *              Create a Habits table if none exists
 *                  Populate Habits table with name of any other existing tables
 *              Create tables for a couple of habits if none exists
 *              Add seed data for tables
 *      
 *      User Interaction
 *          Displays list of habits
 *          User chooses a habit
 *          Options:
 *              View All
 *                  Displays all records for that habit
 *                  Options: Update, Delete, Back
 *                      Update
 *                          Prompt record Id #
 *                          Check if record Id exists
 *                          Prompt new date
 *                          Prompt new quantity
 *                      Delete
 *                          Prompt record Id #
 *              Add
 *                  Allows user to add a record for that habit
 *              Delete All Records
 *                  Clears habit records but keeps table
 *              Delete Habit
 *                  Clears habit records
 *                  Deletes table
 *                  Removes table title from Habits table 
 * 
 * 
 */


/*
 *  Habit Table
 *      Id
 *      HabitName
 *      TableName
 *          Derived from HabitName
 *          HabitName.ToLower()
 *          Get Index of all Spaces
 *          For all spaces greater than 1, set next indexed character to upper
 *          Remove all spaces
 *          GOOD PRACTICE FOR BUILDING A UNIT TEST!!!!
 *  
 */