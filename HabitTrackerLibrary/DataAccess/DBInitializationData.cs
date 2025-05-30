using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerLibrary.DataAccess
{
    /*
     * All this code is obsolete and unused...
     * 
     * 
     * 
     * 
     *  Reminder:
     *      1. Should create DB and tables if not exist
     *      2. Should populate empty tables with seed data
     *      3. Should not populate tables that already have data
     * 
     *  Current State -- Can get it to create the tables and seed Units table with data if empty
     *                   Needs a way to seed data to habits table that links to units table
     *                  
     */


    static public class DBInitializationData
    {

        public static string SqlInitHabitsTable =
        @" CREATE TABLE IF NOT EXISTS Habits
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                UnitsId INTEGER,
                FOREIGN KEY (UnitsId) REFERENCES Units(Id)
            )
        ";

        public static string SqlInitRecordsTable =
        @" CREATE TABLE IF NOT EXISTS Records
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HabitId INTEGER,
                Date TEXT,
                Quantity INTEGER,
                FOREIGN KEY (HabitId) REFERENCES Habits(Id)
            )
        ";

        public static string InitiDataHabits =
        @"  INSERT INTO Units (NameSingle, NamePlural)
            SELECT 'Glass', 'Glasses'
            WHERE NOT EXISTS (SELECT 1 FROM Units WHERE NameSingle = 'Glass');

            INSERT INTO Habits (Name, UnitsId)
            SELECT 'Drinking Water', (select Id from Units where NameSingle = 'Glass')
            
        ";


        // THESE NEED SOME HEAVY WORK!!!!

        // Need to insert into the correct table, but requires the existence of the ocrrent units or habit
        // Maybe separate this...
        //      - Does table exist?
        //              - Yes: Continue
        //              - No: Create Table
        //                      - Add data


        // Starter habits: Drinking Water, Reading Books, 
        public static string InitDataHabits =
            @"  INSERT INTO Habits (Name, UnitsId)
                SELECT 'Unit', 'Units' UNION ALL
                SELECT 'Rep', 'Reps'
            ";

        public static string InitDataRecords =
            @"  INSERT INTO Units (HabitId, Date, Quantity)
                SELECT 'Unit', 'Units' UNION ALL
                SELECT 'Rep', 'Reps'
            ";
    }
}
