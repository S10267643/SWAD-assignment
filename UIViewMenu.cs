using System;
using System.Linq;

namespace SWAD_assignment
{

    public class UIViewMenu
    {
        private readonly CTLViewMenu _controller;

        public UIViewMenu(CTLViewMenu controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public void ShowViewMenu(FoodStallStaff staff)
        {
            bool goBack = false;

            while (!goBack)
            {
                Console.Clear();
                PrintMenuItems(staff);

                Console.WriteLine("\n=== Options ===");
                Console.WriteLine("1. Update Item");
                Console.WriteLine("2. Add Item");
                Console.WriteLine("3. Delete Item");
                Console.WriteLine("0. Go Back");
                Console.Write("Select option: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        DoUpdateItem(staff);
                        Pause();
                        break;
                    case "2":
                        DoAddItem(staff);
                        Pause();
                        break;
                    case "3":
                        DoDeleteItem(staff);
                        Pause();
                        break;
                    case "0":
                        goBack = true; // exit the loop
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Pause();
                        break;
                }
            }
        }

        // ======== Display ========
        private void PrintMenuItems(FoodStallStaff staff)
        {
            var items = _controller.GetMenuForStaff();

            Console.WriteLine($"=== {staff.Stall?.StallName ?? "My Stall"} — Menu ===");
            if (items == null || !items.Any())
            {
                Console.WriteLine("No items found. Use 'Add Item' to create the first one.");
                return;
            }

            foreach (var i in items.OrderBy(i => i.ItemId))
            {
                Console.WriteLine($"\nID: {i.ItemId} | {i.ItemName} | ${i.Price:F2}");
                Console.WriteLine($"Desc: {i.ItemDescription}");
                Console.WriteLine($"Qty: {i.Quantity} | {(i.IsAvailable ? "Available" : "Unavailable")} | Delay: {i.PrepDelayMinutes} min");
            }
        }

        // ======== Update ========
        private void DoUpdateItem(FoodStallStaff staff)
        {
            PrintMenuItems(staff);

            Console.Write("\nEnter Item ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.WriteLine("Invalid ID. Please enter a number.");
                return;
            }

            var current = _controller.GetMenuItemById(itemId);
            if (current == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.WriteLine("\nLeave a field blank to keep the current value.");
            Console.Write($"Name ({current.ItemName}): ");
            string newName = ReadOrNull();

            Console.Write($"Price ({current.Price:F2}): ");
            double? newPrice = ReadDoubleOrNull();

            Console.Write($"Description ({current.ItemDescription}): ");
            string newDesc = ReadOrNull();

            Console.Write($"Quantity ({current.Quantity}): ");
            int? newQty = ReadIntOrNull();

            Console.Write($"Availability [{(current.IsAvailable ? "Y" : "N")}] (Y/N, blank=keep): ");
            bool? newAvail = ReadYesNoOrNull();

            Console.Write($"Prep delay minutes ({current.PrepDelayMinutes}, blank=keep): ");
            int? newDelay = ReadIntOrNull();

            bool ok = _controller.UpdateItem(
                itemId,
                newName,
                newPrice,
                newDesc,
                newQty,
                newAvail,
                newDelay
            );

            Console.WriteLine(ok
                ? "Item updated successfully."
                : "Update failed. Make sure values are valid (e.g., price/qty/delay are not negative, name not empty).");
        }

        // ======== Add ========
        private void DoAddItem(FoodStallStaff staff)
        {
            Console.WriteLine("\n=== Add New Item ===");

            Console.Write("Name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name is required.");
                return;
            }

            Console.Write("Price: ");
            if (!double.TryParse(Console.ReadLine(), out double price) || price < 0)
            {
                Console.WriteLine("Invalid price. Please enter a non-negative number.");
                return;
            }

            Console.Write("Description: ");
            string desc = Console.ReadLine() ?? string.Empty;

            Console.Write("Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
            {
                Console.WriteLine("Invalid quantity. Please enter a non-negative whole number.");
                return;
            }

            bool isAvailable = true;
            Console.Write("Mark available? (Y/N, blank=Y): ");
            string availAns = Console.ReadLine()?.Trim().ToLower();
            if (availAns == "n") isAvailable = false;

            int prepDelay = 0;
            Console.Write("Prep delay minutes? (blank=0): ");
            string delayStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(delayStr))
            {
                if (!int.TryParse(delayStr, out prepDelay) || prepDelay < 0)
                {
                    Console.WriteLine("Invalid delay. Please enter a non-negative number.");
                    return;
                }
            }

            bool ok = _controller.AddItem(name, price, desc, qty, isAvailable, prepDelay);
            Console.WriteLine(ok ? "Item added successfully." : "Add failed. Please check your inputs.");
        }

        // ======== Delete ========
        private void DoDeleteItem(FoodStallStaff staff)
        {
            PrintMenuItems(staff);

            Console.Write("\nEnter Item ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.WriteLine("Invalid ID. Please enter a number.");
                return;
            }

            var item = _controller.GetMenuItemById(itemId);
            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.Write($"Delete '{item.ItemName}'? (Y/N): ");
            string confirm = Console.ReadLine()?.Trim().ToLower();
            if (confirm != "y")
            {
                Console.WriteLine("Delete cancelled.");
                return;
            }

            bool ok = _controller.DeleteItem(itemId);
            Console.WriteLine(ok ? "Item deleted." : "Delete failed.");
        }

        // ======== Small Input Helpers ========
        private static string ReadOrNull()
        {
            string s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private static int? ReadIntOrNull()
        {
            string s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s, out int v) && v >= 0) return v;

            Console.WriteLine("Invalid number. Value will not be updated.");
            return null;
        }

        private static double? ReadDoubleOrNull()
        {
            string s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (double.TryParse(s, out double v) && v >= 0) return v;

            Console.WriteLine("Invalid number. Value will not be updated.");
            return null;
        }

        private static bool? ReadYesNoOrNull()
        {
            string s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().ToLower();

            if (s == "y") return true;
            if (s == "n") return false;

            Console.WriteLine("Invalid input. Value will not be updated.");
            return null;
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }

    public static class ViewMenu
    {
        public static void Show(FoodStallStaff staff)
        {
            var ctl = new CTLViewMenu(staff);
            var ui = new UIViewMenu(ctl);
            ui.ShowViewMenu(staff);
        }
    }

    public static class UpdateMenu
    {
        public static void Show(FoodStallStaff staff)
        {
            var ctl = new CTLViewMenu(staff);
            var ui = new UIViewMenu(ctl);
            ui.ShowViewMenu(staff);
        }
    }
}
