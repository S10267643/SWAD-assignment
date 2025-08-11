using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class FoodStall
    {
        public int StallId { get; set; }
        public string StallName { get; set; }
        public string Description { get; set; }
        public int OperatingHours { get; set; }
        public string Location { get; set; }
        public Menu Menu { get; set; }

        public FoodStall()
        {
            Menu = new Menu();
        }
    }

}
