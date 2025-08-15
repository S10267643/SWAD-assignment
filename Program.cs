
﻿using System;
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
            Menu menu = new Menu();
            menu.AddMenuItem(new MenuItem(1, "Nasi Lemak", 3.50, "Coconut rice with chicken", 10));
            menu.AddMenuItem(new MenuItem(2, "Mee Goreng", 4.00, "Fried noodles", 8));
            menu.AddMenuItem(new MenuItem(3, "Teh Tarik", 1.50, "Milk tea", 15));

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
                            ShowStudentMenu(student, ref loggedInUser, menu);
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
        // Ensure stall has a menu and some starter items for viewing/updating
        static void EnsureStallMenuSeeded(FoodStallStaff staff)
        {
            if (staff.Stall == null)
                staff.Stall = new FoodStall { StallId = new Random().Next(100, 999), StallName = "New Stall" };

            if (staff.Stall.Menu == null)
                staff.Stall.Menu = new Menu();

            var items = staff.Stall.Menu.MenuItems;
            if (items.Count == 0)
            {
                items.Add(new MenuItem(1, "Nasi Lemak", 3.50, "Coconut rice with chicken", 10));
                items.Add(new MenuItem(2, "Mee Goreng", 4.00, "Fried noodles", 8));
                items.Add(new MenuItem(3, "Teh Tarik", 1.50, "Milk tea", 15));
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
            users.Add(new Priority(userCounter++, "Alice Johnson", "alice@student.edu", "student123"));

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
                    Console.WriteLine($"Priority");
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
                        users.Add(new Priority(userCounter++, name, email, password));
                        Console.WriteLine("Priority student account created!");
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

        static void ShowStudentMenu(Student student, ref User loggedInUser, Menu menu)
        {
            // Show notifications first
            student.CheckNotifications();
            // Display priority status if applicable
            if (student is Priority priority)
            {
                Console.WriteLine($"\n=== PRIORITY STUDENT MENU ({priority.Name}) ===");
                Console.WriteLine($"Your benefits: Order Limit: {priority.OrderLimit}, Time Slot: {priority.PickUpTimeSlot}");

            }
            else
            {
                
                Console.WriteLine("\n=== STUDENT MENU ===");
            }

            Console.WriteLine("1. Place Order");
            Console.WriteLine("2. Send Feedback");
            Console.WriteLine("3. View My Feedback");
            Console.WriteLine("4. Logout");
            Console.Write("Select option: ");

            var staffMembers = users.OfType<FoodStallStaff>().ToList();
            var feedbackController = new CTLFeedback(users);
            switch (Console.ReadLine())
            {
                case "1":
                    FoodStall selectedStall = staffMembers[0].Stall;

                    Cart cart = new Cart(student);

                    student.PlaceOrder(menu, selectedStall, cart);
                    break;
                case "2": // Send Feedback
                    student.SubmitFeedback(feedbackController);
                    break;
                 
                case "3":
                    Console.WriteLine("Feature coming soon!");
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

        static void ShowStaffMenu(FoodStallStaff staff, ref User loggedInUser)
        {
            Console.WriteLine("\n=== Staff Menu ===");
            Console.WriteLine($"Managing: {staff.Stall.StallName}");
            Console.WriteLine("1. View All Feedback");          
            Console.WriteLine("2. Respond to Feedback");       
            Console.WriteLine("3. View Incoming Orders");
            Console.WriteLine("4. Update Menu");
            Console.WriteLine("5. View Menu");
            Console.WriteLine("0. Logout");
            Console.Write("Select option: ");

            var feedbackController = new CTLFeedback(users);
            var feedbackUI = new UIFeedback(feedbackController);

            switch (Console.ReadLine())
            {
                case "1": // View All Feedback
                    feedbackUI.ViewAllFeedback(staff);
                    break;

                case "2": // Respond to Feedback
                    feedbackUI.RespondToFeedback(staff, reports);
                    break;
                case "3": // View Incoming Orders
                    if (staff.Stall.Orders.Count == 0)
                    {
                        Console.WriteLine("No incoming orders at the moment.");
                        break;
                    }

                    // Display orders
                    Console.WriteLine("\n=== Current Orders ===");
                    foreach (var o in staff.Stall.Orders)
                    {
                        Console.WriteLine($"ID: {o.OrderId} | Customer: {o.OrderedBy.Name} | Total: {o.TotalPrice:C}");
                    }

                    Console.Write("\nEnter Order ID to manage (0 to cancel): ");
                    if (!int.TryParse(Console.ReadLine(), out int orderId) || orderId == 0)
                    {
                        break;
                    }

                    var order = staff.Stall.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order == null)
                    {
                        Console.WriteLine("Order not found.");
                        break;
                    }

                    // Show order details
                    Console.WriteLine("\n=== Order Details ===");
                    Console.WriteLine($"ID: {order.OrderId}");
                    Console.WriteLine($"Customer: {order.OrderedBy.Name}");
                    Console.WriteLine("Items:");
                    foreach (var item in order.Items)
                    {
                        Console.WriteLine($"- {item.Item.ItemName} x{item.Quantity} @ {item.Item.Price:C}");
                    }
                    Console.WriteLine($"Total: {order.TotalPrice:C}");

                    Console.Write("\nCancel this order? (Y/N): ");
                    if (Console.ReadLine().Trim().ToUpper() == "Y")
                    {
                        staff.Stall.CancelOrder(orderId);
                    }
                    break;

                case "4": // Update Menu
                    EnsureStallMenuSeeded(staff);
                    UpdateMenu.Show(staff);
                    break;

                case "5": // View Menu
                    EnsureStallMenuSeeded(staff);
                    ViewMenu.Show(staff);
                    break;

                case "0": // Logout
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
            while (true)
            {
                Console.WriteLine("\n=== ADMIN MENU ===");
                Console.WriteLine("1. Handle Reports");
                Console.WriteLine("2. Manage Users");
                Console.WriteLine("0. Logout");
                Console.Write("Select option: ");

                var adminsystemUI = new UIAdministratorSystem(admin);
                var adminsystemCTL = new CTLAdministratorSystem(admin);

                switch (Console.ReadLine())
                {
                    case "1": // Handle Reports
                        admin.HandleReports(reports, users);
                        break;

                    case "2": // Manage Users
                        admin.DisplayAllUsers(users);
                        break;

                    case "3": 
                        break;

                    case "4": 
                        break;

                    case "5":
                        break;

                    case "0": // Logout
                        loggedInUser = null;
                        Console.WriteLine("Logged out successfully.");
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }


            static void HandleUserManagement(Administrator admin)
            {
                Console.WriteLine("\n=== USER MANAGEMENT ===");
                Console.WriteLine("All Users:");

                foreach (var user in users)
                {
                    string status = "";
                    if (user is Student s) status = s.SuspensionStatus ? " [SUSPENDED]" : " [ACTIVE]";
                    Console.WriteLine($"{user.UserId}: {user.Name} ({user.Email}) - {user.GetType().Name}{status}");
                }

                Console.WriteLine("\n1. Delete User");
                Console.WriteLine("2. Suspend Student");
                Console.WriteLine("3. Reinstate Student");
                Console.WriteLine("0. Back to Menu");
                Console.Write("Select option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Write("Enter User ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            bool success = admin.DeleteUser(deleteId, users);
                            Console.WriteLine(success ? "User deleted." : "Deletion failed.");
                        }
                        break;
                    case "2":
                        Console.Write("Enter Student ID to suspend: ");
                        if (int.TryParse(Console.ReadLine(), out int suspendId))
                        {
                            bool success = admin.SuspendStudent(suspendId, users);
                            Console.WriteLine(success ? "Student suspended." : "Suspension failed.");
                        }
                        break;
                    case "3":
                        Console.Write("Enter Student ID to reinstate: ");
                        if (int.TryParse(Console.ReadLine(), out int reinstateId))
                        {
                            bool success = admin.ReinstateStudent(reinstateId, users);
                            Console.WriteLine(success ? "Student reinstated." : "Reinstatement failed.");
                        }
                        break;
                }
            }
        }
    }
}

