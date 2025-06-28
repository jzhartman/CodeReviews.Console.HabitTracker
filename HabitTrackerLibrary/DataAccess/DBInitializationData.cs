using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerLibrary.DataAccess
{
    public static class DBInitializationData
    {
        public static void InitializeTables(SqliteDataAccess db)
        {
            InitializeUnitsTable(db);
            SeedUnitsTableData(db);

            InitializeHabitsTable(db);
            SeedHabitsTableData(db);

            InitializeRecordsTable(db);
            SeedRecordsTableData(db);
        }

        private static void InitializeUnitsTable(SqliteDataAccess db)
        {
            db.Execute(
                @"  CREATE TABLE IF NOT EXISTS Units
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT
                    );
                ");
        }

        private static void SeedUnitsTableData(SqliteDataAccess db)
        {
            if (db.RecordExists("Units") == false)
            {
                db.Execute(
                    @"  INSERT INTO Units (Name)
                        SELECT 'Units' UNION ALL
                        SELECT 'Glasses' UNION ALL
                        SELECT 'Pages' UNION ALL
                        SELECT 'kCal' UNION ALL
                        SELECT 'Times' UNION ALL
                        SELECT 'Reps'
                    ");
            }
        }

        private static void InitializeHabitsTable(SqliteDataAccess db)
        {
            db.Execute(
                @" CREATE TABLE IF NOT EXISTS Habits
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        UnitsId INTEGER,
                        FOREIGN KEY (UnitsId) REFERENCES Units(Id)
                    )
                ");
        }

        private static void SeedHabitsTableData(SqliteDataAccess db)
        {
            if (db.RecordExists("Habits") == false)
            {
                db.Execute(GenerateHabitsSeedDataSql("Drinking Water", "Glasses"));
                db.Execute(GenerateHabitsSeedDataSql("Reading", "Pages"));
            }
        }

        private static string GenerateHabitsSeedDataSql(string habitName, string unitName)
        {
            return @$"  INSERT INTO Units (Name)
                        SELECT '{unitName}'
                        WHERE NOT EXISTS (SELECT 1 FROM Units WHERE Name = '{unitName}');

                        INSERT INTO Habits (Name, UnitsId)
                        SELECT '{habitName}', (select Id from Units where Name = '{unitName}')
                        WHERE NOT EXISTS (SELECT 1 FROM Habits WHERE Name = '{habitName}');";
        }

        private static void InitializeRecordsTable(SqliteDataAccess db)
        {
            db.Execute(
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

        private static void SeedRecordsTableData(SqliteDataAccess db)
        {
            if (db.RecordExists("Records") == false)
            {
                int iterations = 100;
                DateTime date = DateTime.Now.AddDays(-iterations);
                Random rnd = new Random();

                SeedHabitsTableData(db);

                for (int i = 0; i < iterations; i++)
                {
                    db.InsertRecordByHabitName("Drinking Water", date.ToString("yyyy-MM-dd"), rnd.Next(1, 25));
                    db.InsertRecordByHabitName("Reading", date.ToString("yyyy-MM-dd"), rnd.Next(1, 100));

                    date = date.AddDays(1);
                }
            }
        }

    }


}
