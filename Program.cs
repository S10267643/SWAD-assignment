using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    class Program
    {
        static List<User> users = new List<User>();
        static List<Report> reports = new List<Report>();
        static int feedbackCounter = 1;
        static int reportCounter = 1;
        static int userCounter = 1;

        public static int GetNextFeedbackId() => feedbackCounter++;
        public static int GetNextReportId() => reportCounter++;

        static void Main(string[] args)
        {
            SeedData();
            User loggedInUser = null;

            while (true)
            {
                if (loggedInUser == null)
                {
                    loggedInUser = ShowLoginMenu();
                }
                else
                {
                    switch (loggedInUser)
                    {
                        case Student student:
                            ShowStudentMenu(student, ref loggedInUser);
                            break;
                        case FoodStallStaff staff:
                            ShowStaffMenu(staff, ref loggedInUser);
                            break;
                        case Administrator admin:
                            ShowAdminMenu(admin, ref loggedInUser);
                            break;
                       
                    }
                }
            }
        }

        static void SeedData()
        {
            // Admin
            users.Add(new Administrator(userCounter++, "Admin", "admin@school.edu", "admin123"));

            // Regular Students
            users.Add(new Student(userCounter++, "John Doe", "john@student.edu", "student123"));
            users.Add(new Student(userCounter++, "Jane Smith", "jane@student.edu", "student123"));

            // Priority Student
            users.Add(new Priority(userCounter++, "Alice Johnson", "alice@student.edu", "student123",
                "Athlete", DateTime.Now.AddMonths(6)));

            // Staff
            var staff1 = new FoodStallStaff(userCounter++, "Mike Johnson", "mike@staff.edu", "staff123");
            staff1.Stall = new FoodStall { StallId = 1, StallName = "Burger King" };
            users.Add(staff1);

            var staff2 = new FoodStallStaff(userCounter++, "Sarah Williams", "sarah@staff.edu", "staff123");
            staff2.Stall = new FoodStall { StallId = 2, StallName = "Pizza Hut" };
            users.Add(staff2);
        }

        static User ShowLoginMenu()
        {
            Console.WriteLine("\n=== Welcome ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create Account");
            Console.WriteLine("3. Exit");
            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    return Login();
                case "2":
                    CreateAccount();
                    return null;
                case "3":
                    Environment.Exit(0);
                    return null;
                default:
                    Console.WriteLine("Invalid option.");
                    return null;
            }
        }

        static User Login()
        {
            Console.Write("\nEmail: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (!User.ValidateCredentials(email, password))
            {
                Console.WriteLine("Invalid email or password format.");
                return null;
            }

            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                              && u.Password == password);
            if (user != null)
            {
                Console.WriteLine($"\nWelcome, {user.Name}!");
                if (user is Priority priority)
                {
                    Console.WriteLine($"Priority Status: {(priority.IsPriorityActive() ? "Active" : "Expired")}");
                }
                return user;
            }

            Console.WriteLine("Invalid credentials.");
            return null;
        }

        static void CreateAccount()
        {
            Console.Write("\nName: ");
            string name = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (!User.ValidateCredentials(email, password))
            {
                Console.WriteLine("Invalid email or password format.");
                return;
            }

            if (!User.IsEmailUnique(users, email))
            {
                Console.WriteLine("Email already in use.");
                return;
            }

            Console.WriteLine("\nAccount Type:");
            Console.WriteLine("1. Student");
            Console.WriteLine("2. Food Stall Staff");
            Console.Write("Select type: ");
            string type = Console.ReadLine();

            switch (type)
            {
                case "1": // Student
                    Console.Write("Is this a priority student? (y/n): ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        Console.Write("Enter priority type (e.g., 'Athlete', 'Disability'): ");
                        string priorityType = Console.ReadLine();
                        Console.Write("Enter expiry date (yyyy-mm-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime expiry))
                        {
                            users.Add(new Priority(userCounter++, name, email, password, priorityType, expiry));
                            Console.WriteLine("Priority student account created!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Creating regular student account.");
                            users.Add(new Student(userCounter++, name, email, password));
                        }
                    }
                    else
                    {
                        users.Add(new Student(userCounter++, name, email, password));
                    }
                    break;
                case "2": // Staff
                    var staff = new FoodStallStaff(userCounter++, name, email, password);
                    staff.Stall = new FoodStall
                    {
                        StallId = new Random().Next(100, 999),
                        StallName = $"{name}'s Stall",
                        Description = "New food stall",
                        Location = "Campus"
                    };
                    users.Add(staff);
                    Console.WriteLine("Staff account created!");
                    break;
                default:
                    Console.WriteLine("Invalid account type.");
                    return;
            }

            Console.WriteLine("Account created successfully!");
        }

        static void ShowStudentMenu(Student student, ref User loggedInUser)
        {
            // Display priority status if applicable
            if (student is Priority priority)
            {
                Console.WriteLine($"\n=== PRIORITY STUDENT MENU ({priority.PriorityType}) ===");
                Console.WriteLine($"Your benefits: Order Limit: {priority.OrderLimit}, Time Slot: {priority.PickUpTimeSlot}");
                Console.WriteLine($"Priority status: {(priority.IsPriorityActive() ? "ACTIVE" : "EXPIRED")}");
            }
            else
            {
                Console.WriteLine("\n=== STUDENT MENU ===");
            }

            Console.WriteLine("1. Send Feedback");
            Console.WriteLine("2. View My Feedback");
            Console.WriteLine("3. Logout");
            Console.Write("Select option: ");

            var staffMembers = users.OfType<FoodStallStaff>().ToList();

            switch (Console.ReadLine())
            {
                case "1":
                    student.SendFeedback(staffMembers);
                    break;
                case "2":
                    Console.WriteLine("Feature coming soon!");
                    break;
                case "3":
                    loggedInUser = null;
                    Console.WriteLine("Logged out successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void ShowStaffMenu(FoodStallStaff staff, ref User loggedInUser)
        {
            Console.WriteLine("\n=== Staff Menu ===");
            Console.WriteLine($"Managing: {staff.Stall.StallName}");
            Console.WriteLine("1. View Feedback");
            Console.WriteLine("2. Report Feedback");
            Console.WriteLine("3. Logout");
            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    if (staff.ReceivedFeedback.Count == 0)
                    {
                        Console.WriteLine("No feedback received yet.");
                        break;
                    }
                    Console.WriteLine("\nReceived Feedback:");
                    foreach (var fb in staff.ReceivedFeedback)
                    {
                        string priorityTag = fb.FromStudent is Priority ? "[PRIORITY] " : "";
                        Console.WriteLine($"- {priorityTag}{fb.Description} (from {fb.FromStudent.Name})");
                    }
                    break;
                case "2":
                    staff.ReportFeedback(reports);
                    break;
                case "3":
                    loggedInUser = null;
                    Console.WriteLine("Logged out successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void ShowAdminMenu(Administrator admin, ref User loggedInUser)
        {
            Console.WriteLine("\n=== Admin Menu ===");
            Console.WriteLine("1. Handle Reports");
            Console.WriteLine("2. View All Users");
            Console.WriteLine("3. Manage Priority Students");
            Console.WriteLine("4. Logout");
            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    admin.HandleReports(reports, users);
                    break;
                case "2":
                    Console.WriteLine("\nAll Users:");
                    foreach (var user in users)
                    {
                        string userType = user.GetType().Name;
                        if (user is Priority p)
                        {
                            userType += $" ({p.PriorityType}, Active: {p.IsPriorityActive()})";
                        }
                        Console.WriteLine($"- {user.Name} ({user.Email}) - {userType}");
                    }
                    break;
                case "3":
                    ManagePriorityStudents();
                    break;
                case "4":
                    loggedInUser = null;
                    Console.WriteLine("Logged out successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void ManagePriorityStudents()
        {
            var priorityStudents = users.OfType<Priority>().ToList();
            if (priorityStudents.Count == 0)
            {
                Console.WriteLine("No priority students found.");
                return;
            }

            Console.WriteLine("\n=== Priority Students ===");
            for (int i = 0; i < priorityStudents.Count; i++)
            {
                var ps = priorityStudents[i];
                Console.WriteLine($"{i + 1}. {ps.Name} - Type: {ps.PriorityType}, " +
                    $"Expires: {ps.PriorityExpiry:d}, Active: {ps.IsPriorityActive()}");
            }

            Console.Write("\nSelect student to modify (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= priorityStudents.Count)
            {
                var student = priorityStudents[choice - 1];
                Console.WriteLine($"\nEditing: {student.Name}");
                Console.WriteLine($"Current Type: {student.PriorityType}");
                Console.WriteLine($"Current Expiry: {student.PriorityExpiry:d}");
                Console.WriteLine($"Current Order Limit: {student.OrderLimit}");
                Console.WriteLine($"Current Time Slot: {student.PickUpTimeSlot}");

                Console.Write("\n1. Extend expiry\n2. Change order limit\n3. Change time slot\nSelect action: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Write("Enter new expiry date (yyyy-mm-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime newExpiry))
                        {
                            student.PriorityExpiry = newExpiry;
                            Console.WriteLine("Expiry date updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                        break;
                    case "2":
                        Console.Write("Enter new order limit: ");
                        if (int.TryParse(Console.ReadLine(), out int newLimit) && newLimit > 0)
                        {
                            student.OrderLimit = newLimit;
                            Console.WriteLine("Order limit updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid limit.");
                        }
                        break;
                    case "3":
                        Console.Write("Enter new time slot (0 = highest priority): ");
                        if (int.TryParse(Console.ReadLine(), out int newSlot) && newSlot >= 0)
                        {
                            student.PickUpTimeSlot = newSlot;
                            Console.WriteLine("Time slot updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid time slot.");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid action.");
                        break;
                }
            }
        }
    }
}