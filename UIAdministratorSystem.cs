using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public class UIAdministratorSystem
    {
        private CTLAdministratorSystem _controller;

        public UIAdministratorSystem(CTLAdministratorSystem controller)
        {
            _controller = controller;
        }

        public void DisplayUserManagementMenu()
        {
            while (true)
            {
                Console.Clear();
                DisplayAllUsers();

                Console.WriteLine("\n=== USER MANAGEMENT ===");
                Console.WriteLine("1. Manage Student Account");
                Console.WriteLine("2. Delete Staff Account");
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("Select option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ManageStudentAccount();
                        break;
                    case "2":
                        DeleteStaffAccount();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option selected.");
                        Pause();
                        break;
                }
            }
        }

        private void DisplayAllUsers()
        {
            var users = _controller.requestUserList();
            Console.WriteLine("\n=== ALL USERS ===");
            foreach (var user in users)
            {
                string status = _controller.getStatus(user);
                Console.WriteLine($"ID: {user.UserId} | Name: {user.Name} | Type: {user.GetType().Name} | Status: {status} | Email: {user.Email}");
            }
        }

        private void ManageStudentAccount()
        {
            Console.Write("\nEnter Student ID to manage: ");
            if (!int.TryParse(Console.ReadLine(), out int studentId))
            {
                Console.WriteLine("Invalid student ID.");
                Pause();
                return;
            }

            var student = _controller.requestUserList().OfType<Student>().FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                Pause();
                return;
            }

            string status = _controller.getStatus(student);
            Console.WriteLine($"\nStudent: {student.Name} (ID: {student.StudentId}) is currently {status}");

            if (status == "Suspended")
            {
                Console.WriteLine("1. Reinstate Student");
                Console.WriteLine("2. Delete Student");
                Console.WriteLine("0. Cancel");
                Console.Write("Select action: ");

                string action = Console.ReadLine();
                switch (action)
                {
                    case "1":
                        ReinstateStudent(student);
                        break;
                    case "2":
                        DeleteUser(student);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                Console.WriteLine("1. Suspend Student");
                Console.WriteLine("2. Delete Student");
                Console.WriteLine("0. Cancel");
                Console.Write("Select action: ");

                string action = Console.ReadLine();
                switch (action)
                {
                    case "1":
                        SuspendStudent(student);
                        break;
                    case "2":
                        DeleteUser(student);
                        break;
                    default:
                        return;
                }
            }
        }

        private void SuspendStudent(Student student)
        {
            Console.Write("Enter reason for suspension: ");
            string reason = Console.ReadLine();

            if (!_controller.inputReason(reason))
            {
                Console.WriteLine("Reason cannot be empty.");
                Pause();
                return;
            }

            student.SuspensionStatus = true;
            student.Notifications.Add($"Your account has been suspended. Reason: {_controller.GetCurrentReason()}");
            Console.WriteLine($"Student {student.Name} has been suspended.");
            Pause();
        }

        private void ReinstateStudent(Student student)
        {
            Console.Write("Enter reason for reinstatement: ");
            string reason = Console.ReadLine();

            if (!_controller.inputReason(reason))
            {
                Console.WriteLine("Reason cannot be empty.");
                Pause();
                return;
            }

            student.SuspensionStatus = false;
            student.Notifications.Add($"Your account has been reinstated. Reason: {_controller.GetCurrentReason()}");
            Console.WriteLine($"Student {student.Name} has been reinstated.");
            Pause();
        }

        private void DeleteStaffAccount()
        {
            Console.Write("\nEnter Staff ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int staffId))
            {
                Console.WriteLine("Invalid staff ID.");
                Pause();
                return;
            }

            var staff = _controller.requestUserList().OfType<FoodStallStaff>().FirstOrDefault(s => s.StaffId == staffId);
            if (staff == null)
            {
                Console.WriteLine("Staff not found.");
                Pause();
                return;
            }

            DeleteUser(staff);
        }

        private void DeleteUser(User user)
        {
            Console.Write($"\nAre you sure you want to delete user ID {user.UserId}? This cannot be undone. (Y/N): ");
            string confirm = Console.ReadLine()?.Trim().ToUpper();

            if (confirm == "Y")
            {
                if (_controller.deleteUser(user))
                {
                    Console.WriteLine("User deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to delete user. You cannot delete your own account.");
                }
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
