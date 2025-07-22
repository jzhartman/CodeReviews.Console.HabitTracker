using HabitTracker;
using HabitTrackerLibrary;
using HabitTrackerLibrary.DataAccess;
using HabitTrackerLibrary.Models;

namespace UnitTest_HabitTrackerReport

{
    [TestClass]
    public sealed class ReportTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            int habitId = 3;
            DateTime startDate = DateTime.Parse("2025-01-01");
            DateTime endDate = DateTime.Parse("2025-07-20");
            SqliteDataAccess db = new SqliteDataAccess("Data Source=HabitTracker.db");
            SqlData sqlData = new SqlData(db, "Data Source=HabitTracker.db");
            var sortedRecords = sqlData.GetAllRecords(habitId).OrderBy(o => o.Date).ToList();
            var report = new ReportModel(sortedRecords, startDate, endDate);


            // Act
            int RecordCount = report.;
            int DayCount = 0;
            double Sum = 0;
            double DailyAverage = 0;
            int StreakDuration = 0;
            double StreakQuantity = 0;
            DateTime StreakStartDate = DateTime.MinValue;


            // Assert
          }
    }
}
