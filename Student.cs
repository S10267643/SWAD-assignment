using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Student : User
    {
        public int StudentId { get; set; }
        public bool SuspensionStatus { get; set; }
        public int OrderLimit { get; set; }
        public int PickUpTimeSlot { get; set; }

        public Student()
        {
        }

        public Student(string password, string name, string email, int studentId,
                       bool suspensionStatus, int orderLimit, int pickUpTimeSlot)
            : base(password, name, email)
        {
            StudentId = studentId;
            SuspensionStatus = suspensionStatus;
            OrderLimit = orderLimit;
            PickUpTimeSlot = pickUpTimeSlot;
        }
    }
}
