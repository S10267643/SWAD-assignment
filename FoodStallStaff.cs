using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public class FoodStallStaff : User
    {
        public int StaffId { get; set; }
        public FoodStall Stall { get; set; }
        public List<Feedback> ReceivedFeedback { get; set; }

        public FoodStallStaff(int userId, string name, string email, string password)
            : base(userId, name, email, password)
        {
            StaffId = userId;
            ReceivedFeedback = new List<Feedback>();
            Stall = new FoodStall();
        }

        public void ReceiveFeedback(Feedback feedback)
        {
            ReceivedFeedback.Add(feedback);
        }

        public void RemoveFeedback(Feedback feedback)
        {
            ReceivedFeedback.Remove(feedback);
        }

        public void ReportFeedback(List<Report> reports)
        {
            if (ReceivedFeedback.Count == 0)
            {
                Console.WriteLine("No feedback available to report.");
                return;
            }

            Console.WriteLine("\nSelect feedback to report:");
            for (int i = 0; i < ReceivedFeedback.Count; i++)
            {
                var fb = ReceivedFeedback[i];
                Console.WriteLine($"{i + 1}. [ID:{fb.FeedbackId}] {fb.Description} (from {fb.FromStudent.Name})");
            }

            if (!int.TryParse(Console.ReadLine(), out int fbChoice) || fbChoice < 1 || fbChoice > ReceivedFeedback.Count)
            {
                Console.WriteLine("Invalid feedback selection.");
                return;
            }

            Console.Write("Enter reason for reporting this feedback: ");
            string reason = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(reason))
            {
                Console.WriteLine("Reason cannot be empty.");
                return;
            }

            var selectedFeedback = ReceivedFeedback[fbChoice - 1];
            var report = new Report(
                Program.GetNextReportId(),
                selectedFeedback.FeedbackId,
                StaffId,
                reason,
                selectedFeedback
            );

            reports.Add(report);
            Console.WriteLine("Feedback reported successfully!");
        }
    }
}