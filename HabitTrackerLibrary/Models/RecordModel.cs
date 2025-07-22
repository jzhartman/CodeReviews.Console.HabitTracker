using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerLibrary.Models
{
    public class RecordModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Quantity { get; set; }
   }
}
