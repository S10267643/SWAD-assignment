using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public class Administrator : User
    {
        public Administrator(int userId, string name, string email, string password)
            : base(userId, name, email, password) { }

        public void HandleReports(List<Report> reports, List<User> users, CTLFeedback feedbackController = null)
        {
            var pendingReports = reports.Where(r => r.Status == "Pending").ToList();

            if (!pendingReports.Any())
            {
                Console.WriteLine("No pending reports to handle.");
                return;
            }

            Console.WriteLine("\n=== Pending Reports ===");
            foreach (var report in pendingReports)
            {
                DisplayReportDetails(report);
                ProcessReportAction(report, users, feedbackController);
            }
        }

        private void DisplayReportDetails(Report report)
        {
            Console.WriteLine($"\nReport ID: {report.ReportId}");
            Console.WriteLine($"Feedback ID: {report.FeedbackId}");
            Console.WriteLine($"Reported by Staff ID: {report.ReportedByStaffId}");
            Console.WriteLine($"Reason: {report.Reason}");
            Console.WriteLine($"Date Reported: {report.DateReported:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Feedback Content: \"{report.Feedback.Description}\"");
            Console.WriteLine($"From Student: {report.Feedback.FromStudent.Name}");

            if (report.Feedback.FromStudent is Priority)
            {
                Console.WriteLine("Note: This feedback is from a PRIORITY student");
            }

            Console.WriteLine(new string('=', 50));
        }

        private void ProcessReportAction(Report report, List<User> users, CTLFeedback feedbackController)
        {
            Console.WriteLine("\nAction options:");
            Console.WriteLine("1. Approve Report (Remove feedback)");
            Console.WriteLine("2. Reject Report (Keep feedback)");
            Console.WriteLine("3. Skip to next");
            Console.Write("Select action (1-3): ");

            if (!int.TryParse(Console.ReadLine(), out int action) || action < 1 || action > 3)
            {
                Console.WriteLine("Invalid action. Skipping report.");
                return;
            }

            switch (action)
            {
                case 1:
                    ApproveReport(report, users, feedbackController);
                    break;
                case 2:
                    report.Reject();
                    Console.WriteLine("✗ Report rejected. Feedback remains.");
                    break;
                case 3:
                    Console.WriteLine("Report skipped.");
                    break;
            }
        }

        private void ApproveReport(Report report, List<User> users, CTLFeedback feedbackController)
        {
            var staff = users.OfType<FoodStallStaff>()
                           .FirstOrDefault(s => s.StaffId == report.ReportedByStaffId);

            if (staff == null)
            {
                Console.WriteLine("Staff member not found. Report cannot be processed.");
                return;
            }

            // Always use feedbackController if available
            if (feedbackController != null)
            {
                bool success = feedbackController.DeleteFeedback(report.Feedback.FeedbackId);
                if (!success)
                {
                    Console.WriteLine("Failed to delete feedback. It may have already been removed.");
                    return;
                }
            }
            else
            {
                // Fallback to direct removal if controller isn't available
                staff.RemoveFeedback(report.Feedback);
            }

            report.Approve();
            Console.WriteLine("✓ Report approved. Feedback has been removed.");

            // Notify the student whose feedback was removed
            report.Feedback.FromStudent.Notifications.Add(
                $"Your feedback to {staff.Stall?.StallName ?? "a food stall"} was removed by an administrator.");
        }
    }
}