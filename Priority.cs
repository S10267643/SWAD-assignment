using System;

namespace SWAD_assignment
{
    public class Priority : Student
    {
        
        public int PriorityOrderLimit { get; set; }
        public int PriorityPickUpTimeSlot { get; set; }
        

        public Priority(int userId, string name, string email, string password )
            : base(userId, name, email, password)
        {
            
            PriorityOrderLimit = 10;            
            PriorityPickUpTimeSlot = 5;          
            

            // Override base properties
            OrderLimit = PriorityOrderLimit;
            PickUpTimeSlot = PriorityPickUpTimeSlot;
        }

       

        

        
    }
}