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

            if (!pendingReports.Any())
            {
                Console.WriteLine("No pending reports to handle.");
                return;
            }

            Console.WriteLine("\n=== Pending Reports ===");
            foreach (var report in pendingReports)
            {
                DisplayReportDetails(report);
                ProcessReportAction(report, users);
            }
            Console.Write("Select report: ");
        }

        private void DisplayReportDetails(Report report)
        {
            Console.WriteLine($"\nReport ID: {report.ReportId}");
            Console.WriteLine($"Feedback ID: {report.FeedbackId}");
            Console.WriteLine($"Reported by Staff ID: {report.ReportedByStaffId}");
            Console.WriteLine($"Reason: {report.Reason}");
            Console.WriteLine($"Feedback Content: {report.Feedback.Description}");
            Console.WriteLine($"From Student: {report.Feedback.FromStudent.Name}");
        }

        private void ProcessReportAction(Report report, List<User> users)
        {
            Console.Write("\nAction: (1) Approve, (2) Reject, (3) Skip: ");
            Console.Write("Select option: ");
            if (!int.TryParse(Console.ReadLine(), out int action) || action < 1 || action > 3)
            {
                Console.WriteLine("Invalid action. Skipping report.");
                return;
            }

            switch (action)
            {
                case 1:
                    ApproveReport(report, users);
                    break;
                case 2:
                    report.Reject();
                    Console.WriteLine("Report rejected. Feedback remains.");
                    break;
                case 3:
                    Console.WriteLine("Report skipped.");
                    break;
            }
        }

        private void ApproveReport(Report report, List<User> users)
        {
            var staff = users.OfType<FoodStallStaff>()
                           .FirstOrDefault(s => s.StaffId == report.ReportedByStaffId);

            if (staff == null)
            {
                Console.WriteLine("Staff member not found. Report cannot be processed.");
                return;
            }

            staff.RemoveFeedback(report.Feedback);
            report.Approve();
            Console.WriteLine("Report approved. Feedback removed.");
        }

        public bool DeleteUser(int userId, List<User> users)
        {
            if (userId == this.UserId)
            {
                Console.WriteLine("Cannot delete your own account.");
                return false;
            }

            var userToDelete = users.FirstOrDefault(u => u.UserId == userId);
            if (userToDelete == null)
            {
                Console.WriteLine("User not found.");
                return false;
            }

            users.Remove(userToDelete);
            Console.WriteLine($"User {userToDelete.Name} (ID: {userToDelete.UserId}) deleted successfully.");
            return true;
        }

        public bool SuspendStudent(int studentId, List<User> users)
        {
            var student = users.OfType<Student>().FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return false;
            }

            if (student.SuspensionStatus)
            {
                Console.WriteLine("Student is already suspended.");
                return false;
            }

            student.SuspensionStatus = true;
            Console.WriteLine($"Student {student.Name} (ID: {student.StudentId}) suspended.");
            return true;
        }

        public bool ReinstateStudent(int studentId, List<User> users)
        {
            var student = users.OfType<Student>().FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return false;
            }

            if (!student.SuspensionStatus)
            {
                Console.WriteLine("Student is not currently suspended.");
                return false;
            }

            student.SuspensionStatus = false;
            Console.WriteLine($"Student {student.Name} (ID: {student.StudentId}) reinstated.");
            return true;
        }

        public void DisplayAllUsers(List<User> users)
        {
            Console.WriteLine("\n=== All Users ===");
            foreach (var user in users)
            {
                string userType = user.GetType().Name;
                string status = "";

                if (user is Student student)
                {
                    status = student.SuspensionStatus ? " [SUSPENDED]" : " [ACTIVE]";
                }

                Console.WriteLine($"ID: {user.UserId} | Name: {user.Name} | Type: {userType}{status} | Email: {user.Email}");
            }
        }
    }
}