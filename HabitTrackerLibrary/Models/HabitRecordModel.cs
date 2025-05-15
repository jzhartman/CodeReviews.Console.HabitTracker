using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerLibrary.Models
{
    public class HabitRecordModel
    {
        public int Id { get; set; }
        public string HabitId { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
   }
}
