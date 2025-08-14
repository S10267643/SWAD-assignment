namespace SWAD_assignment
{
    public class UIFeedback
    {
        private CTLFeedback _controller;

        public UIFeedback(CTLFeedback controller)
        {
            _controller = controller;
        }

        public void ViewAllFeedback(FoodStallStaff staff)
        {
            var feedbacks = _controller.GetFeedbacksForStaff(staff.StaffId);

            if (!feedbacks.Any())
            {
                Console.WriteLine("No feedback received yet.");
                return;
            }

            Console.WriteLine($"\n=== Feedback for {staff.Stall.StallName} ===");
            foreach (var fb in feedbacks)
            {
                Console.WriteLine($"\nID: {fb.FeedbackId}");
                Console.WriteLine($"From: {fb.FromStudent.Name}");
                Console.WriteLine($"Date: {fb.DateCreated:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Content: {fb.Description}");

                if (fb.HasResponse)
                {
                    Console.WriteLine($"Response: {fb.Response}");
                    Console.WriteLine($"Response Date: {fb.ResponseDate:yyyy-MM-dd HH:mm}");
                }
                else
                {
                    Console.WriteLine("Status: Awaiting response");
                }

                Console.WriteLine(new string('-', 50));
            }
        }

        public void RespondToFeedback(FoodStallStaff staff, List<Report> reports)
        {
            var feedbacks = _controller.GetFeedbacksForStaff(staff.StaffId)
                .Where(f => !f.IsReported)  // Changed to show all non-reported feedback
                .ToList();

            if (!feedbacks.Any())
            {
                Console.WriteLine("No feedback available to respond to.");
                return;
            }

            Console.WriteLine("\n=== Select Feedback to Manage ===");
            for (int i = 0; i < feedbacks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID:{feedbacks[i].FeedbackId} From: {feedbacks[i].FromStudent.Name}");
                Console.WriteLine($"   \"{feedbacks[i].Description}\"");
                if (feedbacks[i].HasResponse)
                {
                    Console.WriteLine($"   Response: \"{feedbacks[i].Response}\"");
                }
            }

            Console.Write($"Select feedback (1-{feedbacks.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > feedbacks.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var selectedFeedback = feedbacks[choice - 1];

            Console.WriteLine($"\nManaging feedback from {selectedFeedback.FromStudent.Name}:");
            Console.WriteLine($"\"{selectedFeedback.Description}\"");

            if (selectedFeedback.HasResponse)
            {
                Console.WriteLine($"\nExisting Response: \"{selectedFeedback.Response}\"");
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Respond to feedback");
            Console.WriteLine("2. Report feedback");
            Console.WriteLine("3. Cancel");
            Console.Write("Select option: ");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1": // Respond
                    if (selectedFeedback.HasResponse)
                    {
                        Console.WriteLine("This feedback already has a response.");
                        break;
                    }

                    Console.Write("\nEnter your response: ");
                    string response = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        Console.WriteLine("Response cannot be empty.");
                        return;
                    }

                    _controller.SubmitResponse(staff.StaffId, selectedFeedback.FeedbackId, response);
                    Console.WriteLine("Response submitted successfully!");
                    break;

                case "2": // Report
                    if (selectedFeedback.IsReported)
                    {
                        Console.WriteLine("This feedback has already been reported.");
                        break;
                    }

                    Console.Write("\nEnter reason for reporting: ");
                    string reason = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        Console.WriteLine("Reason cannot be empty.");
                        return;
                    }

                    _controller.ReportFeedback(staff.StaffId, selectedFeedback.FeedbackId, reason, reports);
                    Console.WriteLine("Feedback reported successfully! An administrator will review it.");
                    break;

                case "3": // Cancel
                    Console.WriteLine("Operation cancelled.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}