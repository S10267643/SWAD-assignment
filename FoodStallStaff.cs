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

        public void RespondToFeedback(int feedbackId, string response)
        {
            var feedback = ReceivedFeedback.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (feedback != null && !feedback.HasResponse)
            {
                feedback.Response = response;
                feedback.ResponseDate = DateTime.Now;

                // Notify student
                feedback.FromStudent.Notifications.Add(
                    $"Your feedback to {Stall.StallName} has been responded to: \"{response}\"");
            }
        }

        public void ReportFeedback(Feedback feedback, string reason, List<Report> reports)
        {
            if (feedback != null && !feedback.IsReported)
            {
                feedback.IsReported = true;
                var report = new Report(
                    Program.GetNextReportId(),
                    feedback.FeedbackId,
                    StaffId,
                    reason,
                    feedback
                );
                reports.Add(report);
            }
        }

        public void ViewIncomingOrders()
        {
            Stall.DisplayAllOrders();
        }

        public void CancelOrder()
        {
            if (Stall.Orders.Count == 0)
            {
                Console.WriteLine("No orders to cancel.");
                return;
            }

            Stall.DisplayAllOrders();
            Console.Write("\nEnter Order ID to cancel: ");

            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid order ID.");
                return;
            }

            Console.Write($"Are you sure you want to cancel Order #{orderId}? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                Stall.CancelOrder(orderId);
            }
            else
            {
                Console.WriteLine("Cancellation cancelled.");
            }
        }
    }
}