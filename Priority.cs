using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Priority : Student
    {
        public int PriorityOrderLimit { get; set; }
        public int PriorityPickUpTimeSlot { get; set; }

        public Priority()
        {
        }

        public Priority(int priorityOrderLimit, int priorityPickUpTimeSlot)
        {
            PriorityOrderLimit = priorityOrderLimit;
            PriorityPickUpTimeSlot = priorityPickUpTimeSlot;
        }
    }
}
