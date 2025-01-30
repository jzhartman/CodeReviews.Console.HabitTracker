using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    internal class DataAccess
    {
        private readonly string _connectionString;

        internal DataAccess(string connectionString)
        {
            this._connectionString = connectionString;
        }
        //internal void Update()
        //{
        //    Console.Clear();
        //    GetAllRecords();

        //    var recordId = GetNumberInput("Enter Id of the record you would like to update or type 0 to return to the main menu: ");
        //    ReturnToMainMenu(recordId.ToString());

        //    using (var connection = new SqliteConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var checkCmd = connection.CreateCommand();
        //        checkCmd.CommandText = $"select exists (Select 1 from drinkingWater where Id = {recordId}";
        //        int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

        //        if (checkQuery == 0)
        //        {
        //            Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
        //            connection.Close();
        //            UpdateRecord();
        //        }


        //        DateOnly date = GetDateInput("Please insert the date (Format yyyy-mm-dd): ");
        //        int quantitiy = GetNumberInput("Please insert number of units (no decimals allowed): ");

        //        var tableCmd = connection.CreateCommand();
        //        tableCmd.CommandText = $"update drinkingWater set Date = '{date.ToString("yyyy-MM-dd")}', quantity = '{quantitiy}' where Id = {recordId}";

        //        tableCmd.ExecuteNonQuery();
        //        connection.Close();
        //    }
        //}

        //internal void Delete()
        //{
        //    Console.Clear();
        //    GetAllRecords();

        //    var recordId = GetNumberInput("Enter the Id of the record you want to delete or type 0 to return to the Main Menu: ");
        //    ReturnToMainMenu(recordId.ToString());

        //    using (var connection = new SqliteConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var tableCmd = connection.CreateCommand();
        //        tableCmd.CommandText = $"delete from drinkingWater where id = '{recordId}'";

        //        int rowCount = tableCmd.ExecuteNonQuery();

        //        if (rowCount == 0)
        //        {
        //            Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
        //            DeleteRecord();
        //        }
        //        Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");
        //    }
        //}

        internal void Insert(DateOnly date, int quantity)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"insert into drinkingWater(date, quantity) values('{date}', '{quantity}')";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }


        internal void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "select * from drinkingWater";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2),
                            }
                         );
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                Console.WriteLine("--------------------------------------------------------\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy")} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("--------------------------------------------------------\n");

            }
        }

    }
}
