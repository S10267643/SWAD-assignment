using System;

namespace SWAD_assignment
{
    public class Priority : Student
    {
        public string PriorityType { get; set; }
        public int ExtendedOrderLimit { get; set; }
        public int PriorityPickUpTimeSlot { get; set; }
        public DateTime PriorityExpiry { get; set; }

        public Priority(int userId, string name, string email, string password,
                      string priorityType, DateTime expiryDate)
            : base(userId, name, email, password)
        {
            PriorityType = priorityType;
            ExtendedOrderLimit = 10;             // Higher limit for priority students
            PriorityPickUpTimeSlot = 0;          // 0 = highest priority slot
            PriorityExpiry = expiryDate;

            // Override base properties
            OrderLimit = ExtendedOrderLimit;
            PickUpTimeSlot = PriorityPickUpTimeSlot;
        }

        public bool IsPriorityActive()
        {
            return DateTime.Now <= PriorityExpiry && !SuspensionStatus;
        }

        public void UpdatePriority(int newLimit, int newTimeSlot, DateTime newExpiry)
        {
            if (newLimit > 0) ExtendedOrderLimit = newLimit;
            if (newTimeSlot >= 0) PriorityPickUpTimeSlot = newTimeSlot;
            PriorityExpiry = newExpiry;

            // Update current values if priority is active
            if (IsPriorityActive())
            {
                OrderLimit = ExtendedOrderLimit;
                PickUpTimeSlot = PriorityPickUpTimeSlot;
            }
        }

        public override string ToString()
        {
            string status = IsPriorityActive() ? "Active" : "Expired";
            return $"{Name} (Priority: {PriorityType}, Status: {status}, " +
                   $"Order Limit: {OrderLimit}, Time Slot: {PickUpTimeSlot})";
        }
    }
}