namespace SWAD_assignment
{
    public class Student : User
    {
        public int StudentId { get; set; }
        public bool SuspensionStatus { get; set; }
        public int OrderLimit { get; set; }
        public int PickUpTimeSlot { get; set; }

        public List<string> Notifications { get; set; } = new List<string>();

        public Student(int userId, string name, string email, string password)
             : base(userId, name, email, password)
        {
            StudentId = userId;
            StudentId = userId;
            SuspensionStatus = false;
            OrderLimit = 3;          // Default order limit
            PickUpTimeSlot = 15;
        }
        public void SubmitFeedback(CTLFeedback feedbackController)
        {
            if (SuspensionStatus)
            {
                Console.WriteLine("Your account is suspended and cannot send feedback.");
                return;
            }

            var staffMembers = feedbackController.GetAllStaff();
            if (!staffMembers.Any())
            {
                Console.WriteLine("No food stalls available for feedback.");
                return;
            }

            Console.WriteLine("\n=== Submit Feedback ===");
            Console.WriteLine("Select a food stall:");

            for (int i = 0; i < staffMembers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {staffMembers[i].Stall?.StallName ?? "Unknown Stall"}");
            }

            Console.Write($"Enter selection (1-{staffMembers.Count}, or 0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > staffMembers.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            if (choice == 0)
            {
                Console.WriteLine("Feedback submission cancelled.");
                return;
            }

            var selectedStaff = staffMembers[choice - 1];

            Console.Write("Enter your feedback: ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Feedback cannot be empty.");
                return;
            }

            bool success = feedbackController.SubmitFeedback(this.StudentId, selectedStaff.StaffId, description);
            Console.WriteLine(success ? "Feedback submitted successfully!" : "Failed to submit feedback.");
        }

        public void CheckNotifications()
        {
            if (Notifications.Count == 0) return;

            Console.WriteLine("\n=== Notifications ===");
            foreach (var notification in Notifications)
            {
                Console.WriteLine($"- {notification}");
            }
            Notifications.Clear();
        }
        public void LastMinuteSlot(FoodStall stall)
        {
            Console.WriteLine("\n⚠️  A last minute change will result in a $2 fee. Proceed? (Y/N)");
            string confirm = Console.ReadLine()?.Trim().ToUpper();

            if (confirm != "Y")
            {
                Console.WriteLine("Understood. Returning to dashboard.");
                return;
            }

            // Display available time slots
            stall.DisplayTimeSlots();

            Console.Write("\nPlease select a new time slot by ID: ");
            if (!int.TryParse(Console.ReadLine(), out int slotId))
            {
                Console.WriteLine("Invalid slot selection.");
                return;
            }

            // Validate the selected time slot is in the last-minute window (1:30 PM - 2:15 PM)
            // Assuming slot IDs 16-18 correspond to these times as in your original code
            if (slotId < 16 || slotId > 18)
            {
                Console.WriteLine("Invalid last-minute slot selection. Only slots 16-18 are available for last-minute changes.");
                return;
            }

            // Check if the slot is actually available
            if (!stall.IsTimeSlotAvailable(slotId))
            {
                Console.WriteLine("This time slot is not available. Please choose another.");
                return;
            }

            Console.WriteLine("\nLate Change Fee: $2");
            Console.WriteLine("Would you like to proceed with this change? (Y/N)");
            confirm = Console.ReadLine()?.Trim().ToUpper();

            if (confirm == "Y")
            {
                if (stall.BookTimeSlot(slotId))
                {
                    // Get the time for display
                    var slotTime = stall.GetTimeSlotTime(slotId);

                    Console.WriteLine("\nTime slot successfully booked!");
                    Console.WriteLine($"New pick up time: {slotTime.ToString("hh:mm tt")}");
                    Console.WriteLine("$2 late change fee applied");
                }
                else
                {
                    Console.WriteLine("Failed to book the time slot. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Change cancelled. Returning to dashboard.");
            }
        }
    }
}
