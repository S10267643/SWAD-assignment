using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public class Administrator : User
    {
        public Administrator(int userId, string name, string email, string password)
            : base(userId, name, email, password) { }

        public void HandleReports(List<Report> reports, List<User> users)
        {
            var pendingReports = reports.Where(r => r.Status == "Pending").ToList();

            if (pendingReports.Count == 0)
            {
                Console.WriteLine("No pending reports to handle.");
                return;
            }

            Console.WriteLine("\n=== Pending Reports ===");
            foreach (var report in pendingReports)
            {
                Console.WriteLine($"\nReport ID: {report.ReportId}");
                Console.WriteLine($"Feedback ID: {report.FeedbackId}");
                Console.WriteLine($"Reported by Staff ID: {report.ReportedByStaffId}");
                Console.WriteLine($"Reason: {report.Reason}");
                Console.WriteLine($"Feedback Content: {report.Feedback.Description}");
                Console.WriteLine($"From Student: {report.Feedback.FromStudent.Name}");

                Console.Write("\nAction: (1) Approve, (2) Reject, (3) Skip: ");
                if (!int.TryParse(Console.ReadLine(), out int action) || action < 1 || action > 3)
                {
                    Console.WriteLine("Invalid action.");
                    continue;
                }

                switch (action)
                {
                    case 1: // Approve
                        var staff = users.OfType<FoodStallStaff>()
                            .FirstOrDefault(s => s.StaffId == report.ReportedByStaffId);
                        staff?.RemoveFeedback(report.Feedback);
                        report.Approve();
                        Console.WriteLine("Report approved. Feedback removed.");
                        break;
                    case 2: // Reject
                        report.Reject();
                        Console.WriteLine("Report rejected. Feedback remains.");
                        break;
                    case 3: // Skip
                        Console.WriteLine("Report skipped.");
                        break;
                }
            }
        }
    }
}