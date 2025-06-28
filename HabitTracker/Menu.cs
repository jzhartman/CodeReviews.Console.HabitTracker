using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    internal class Menu
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> Options { get; set; }

        public Menu(string title, string message, List<string> options) 
        {
            this.Title = title;
            this.Message = message;
            this.Options = options;
        }

        public void PrintMenu()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine(Message);
            Console.WriteLine();

            PrintOptions();
            Console.WriteLine();

            Console.WriteLine("---------------------------------------------------------");
        }

        private void PrintOptions()
        {
            int i = 1;

            foreach (string option in Options)
            {
                Console.WriteLine($"\t{i}: {option}");
                i++;
            }
        }
    }
}
