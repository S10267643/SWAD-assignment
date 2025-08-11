namespace SWAD_assignment
{
    public class Student : User
    {
        public int StudentId { get; set; }
        public bool SuspensionStatus { get; set; }
        public int OrderLimit { get; set; }
        public int PickUpTimeSlot { get; set; }

        public Student(int userId, string name, string email, string password)
             : base(userId, name, email, password)
        {
            StudentId = userId;
            SuspensionStatus = false;
            OrderLimit = 5;          // Default order limit
            PickUpTimeSlot = 1;
        }
        public void SendFeedback(List<FoodStallStaff> stalls)
        {
            if (SuspensionStatus)
            {
                Console.WriteLine("Your account is suspended and cannot send feedback.");
                return;
            }

            Console.WriteLine("\nSelect a stall to send feedback to:");
            for (int i = 0; i < stalls.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {stalls[i].Stall.StallName}");
            }

            if (!int.TryParse(Console.ReadLine(), out int stallChoice) || stallChoice < 1 || stallChoice > stalls.Count)
            {
                Console.WriteLine("Invalid stall selection.");
                return;
            }

            Console.Write("Enter your feedback: ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Feedback cannot be empty.");
                return;
            }

            var feedback = new Feedback
            {
                FeedbackId = Program.GetNextFeedbackId(),
                Description = description,
                FromStudent = this
            };

            stalls[stallChoice - 1].ReceiveFeedback(feedback);
            Console.WriteLine("Feedback sent successfully!");
        }
    }
}