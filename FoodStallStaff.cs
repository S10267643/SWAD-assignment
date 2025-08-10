using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class FoodStallStaff : User
    {
        public int StaffId { get; set; }

        public FoodStallStaff()
        {
        }

        public FoodStallStaff(int staffId)
        {
            StaffId = staffId;
        }
    }
}
