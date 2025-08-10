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
        public List<Menu> Menus { get; set; }

        public FoodStall()
        {
            Menus = new List<Menu>();
        }

        public FoodStall(int stallId, string stallName, string description,
                         int operatingHours, string location)
        {
            StallId = stallId;
            StallName = stallName;
            Description = description;
            OperatingHours = operatingHours;
            Location = location;
            Menus = new List<Menu>();
        }
    }
}
