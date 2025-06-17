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

        /*  
         *  INTITIALIZE ALL TABLES
         *  
         */


        // DB created when app runs -- base seed data on empty tables

        public void InitializeTables()
        {
            InitializeUnitsTable();
            InitializeHabitsTable();
            InitializeRecordsTable();

            if (RecordExists("Records") == false)
            {
                //Execute(DBInitializationData.InitDataRecords);
            }
        }

        public void InitializeUnitsTable()
        {
            Execute(
                @"  CREATE TABLE IF NOT EXISTS Units
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT
                    );
                ");

            SeedUnitsTableData();
        }

        public void SeedUnitsTableData()
        {
            if (RecordExists("Units") == false)
            {
                Execute(
                    @"  INSERT INTO Units (Name)
                        SELECT 'Unit', 'Units' UNION ALL
                        SELECT 'Glass', 'Glasses' UNION ALL
                        SELECT 'Book', 'Books' UNION ALL
                        SELECT 'kCal', 'kCal' UNION ALL
                        SELECT 'Time', 'Times' UNION ALL
                        SELECT 'Rep', 'Reps'
                    ");
            }
        }

        public void InitializeHabitsTable()
        {
            Execute(
                @" CREATE TABLE IF NOT EXISTS Habits
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        UnitsId INTEGER,
                        FOREIGN KEY (UnitsId) REFERENCES Units(Id)
                    )
                ");

            SeedHabitsTableData();
        }

        public void SeedHabitsTableData()
        {
            if (RecordExists("Habits") == false)
            {
                Execute(GenerateHabitsSeedDataSql("Drinking Water", "Glasses"));
                Execute(GenerateHabitsSeedDataSql("Reading Books", "Books"));
            }
        }

        public string GenerateHabitsSeedDataSql(string habitName, string unitName)
        {
            return @$"  INSERT INTO Units (Name)
                        SELECT '{unitName}'
                        WHERE NOT EXISTS (SELECT 1 FROM Units WHERE Name = '{unitName}');

                        INSERT INTO Habits (Name, UnitsId)
                        SELECT '{habitName}', (select Id from Units where Name = '{unitName}')
                        WHERE NOT EXISTS (SELECT 1 FROM Habits WHERE Name = '{habitName}');";
        }

        public void InitializeRecordsTable()
        {
            Execute(
                @" CREATE TABLE IF NOT EXISTS Records
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        HabitId INTEGER,
                        Date TEXT,
                        Quantity INTEGER,
                        FOREIGN KEY (HabitId) REFERENCES Habits(Id)
                    )
                ");
        }

        public void GenerateRecordsSeedData()
        {
            int iterations = 10;
            DateTime date = DateTime.Now.AddDays(-iterations);
            Random rnd = new Random();

            SeedHabitsTableData();

            for (int i = 0; i < iterations; i++)
            {
                //InsertRecord("Habits", "Drinking Water", date.ToString("yyyy-MM-dd"), rnd.Next(1, 25));
                //InsertRecord("Habits", "Reading", date.ToString("yyyy-MM-dd"), rnd.Next(1, 5));

                date.AddDays(1);
            }
        }

        public void InsertRecord(string tableName, int habitId, string date, int quantity)
        {
            Execute($"insert into {tableName}(HabitId, Date, Quantity) values({habitId}, '{date}', '{quantity}')");
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

        public bool CheckIfHabitExists(string habitName)
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM Habits WHERE Name = '{habitName}' COLLATE NoCase)";
                var test = tableCmd.ExecuteScalar();


                bool habitExists = Convert.ToBoolean(test);

                connection.Close();

                return habitExists;
            }
        }

        public bool CheckIfUnitExists(string unitName)
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM Units WHERE Name = '{unitName}' COLLATE NoCase)";

                bool unitExists = Convert.ToBoolean(tableCmd.ExecuteScalar());

                connection.Close();

                return unitExists;
            }
        }

        public HabitModel GetHabitByName(string name)
        {
            throw new NotImplementedException();
        }


        public List<RecordModel> GetAllRecords(int habitId)
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"select Id, Date, Quantity from Records where records.HabitId = {habitId}";

                List<RecordModel> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new RecordModel
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

        public List<HabitModel> GetAllHabits()
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"select habits.Id as HabitId, habits.Name as HabitName, Units.Name as UnitsName
                                          from habits
                                          inner join units on habits.UnitsId = Units.Id";

                List<HabitModel> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new HabitModel
                            {
                                HabitId = reader.GetInt32(0),
                                HabitName = reader.GetString(1),
                                UnitName = reader.GetString(2),
                            }
                         );
                    }
                }

                connection.Close();

                return tableData;
            }
        }

        public List<UnitModel> GetAllUnits()
        {
            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"select units.Id as UnitId, units.Name as UnitName from units";

                List<UnitModel> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new UnitModel
                            {
                                UnitId = reader.GetInt32(0),
                                UnitName = reader.GetString(1),
                            }
                         );
                    }
                }

                connection.Close();

                return tableData;
            }
        }

        public bool RecordExists(string tableName, int recordId = -1)
        {
            bool recordExists = false;

            using (var connection = new SqliteConnection(connectionStringName))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();

                if(recordId < 0)
                {
                    checkCmd.CommandText = $"select exists (Select 1 from {tableName})";
                }
                else
                {
                    checkCmd.CommandText = $"select exists (Select 1 from {tableName} where Id = {recordId}";

                }

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