/// UpdateMenu.cs
using System;
using System.Collections.Generic;

namespace SWAD_assignment
{
    public static class UpdateMenu
    {
        public static void Show(FoodStallStaff staff)
        {
            if (staff == null || staff.Stall == null)
            {
                Console.WriteLine("Error: No staff/stall context.");
                return;
            }
            if (staff.Stall.Menu == null)
                staff.Stall.Menu = new Menu();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("==============================================");
                Console.WriteLine(" Update Menu - " + staff.Stall.StallName);
                Console.WriteLine("==============================================");
                Console.WriteLine("1) Update existing item");
                Console.WriteLine("2) Add new item");
                Console.WriteLine("3) Delete item");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        UpdateExistingItem(staff.Stall.Menu.MenuItems);
                        break;
                    case "2":
                        AddNewItem(staff.Stall.Menu.MenuItems);
                        break;
                    case "3":
                        DeleteItem(staff.Stall.Menu.MenuItems);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // ===== Update =====
        private static void UpdateExistingItem(List<MenuItem> items)
        {
            if (!EnsureHasItems(items)) return;

            PrintTable(items);

            Console.Write("\nEnter Item ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            // find by id without LINQ
            MenuItem item = null;
            foreach (var it in items)
            {
                if (it.ItemId == id) { item = it; break; }
            }
            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.WriteLine("\nCurrent values (leave blank to keep the current value):");
            Console.WriteLine("Name            : " + item.ItemName);
            Console.WriteLine("Price           : " + item.Price.ToString("0.00"));
            Console.WriteLine("Quantity        : " + item.Quantity);
            Console.WriteLine("Description     : " + item.ItemDescription);
            Console.WriteLine("Availability    : " + (item.IsAvailable ? "Available" : "Unavailable"));
            Console.WriteLine("Prep Delay (min): " + item.PrepDelayMinutes);

            // Name
            Console.Write("New name: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                item.ItemName = name.Trim();

            // Price
            Console.Write("New price: ");
            string priceStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceStr))
            {
                if (double.TryParse(priceStr, out double price) && price >= 0)
                    item.Price = price;
                else
                {
                    Console.WriteLine("Error: Price must be a number >= 0.");
                    return;
                }
            }

            // Quantity
            Console.Write("New quantity: ");
            string qtyStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(qtyStr))
            {
                if (int.TryParse(qtyStr, out int qty) && qty >= 0)
                    item.Quantity = qty;
                else
                {
                    Console.WriteLine("Error: Quantity must be an integer >= 0.");
                    return;
                }
            }

            // Description
            Console.Write("New description: ");
            string desc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(desc))
                item.ItemDescription = desc.Trim();

            // Availability
            Console.Write("Change availability? (y/n, blank = no change): ");
            string avail = (Console.ReadLine() ?? "").Trim().ToLower();
            if (avail == "y" || avail == "yes") item.IsAvailable = true;
            else if (avail == "n" || avail == "no") item.IsAvailable = false;

            // Prep delay
            Console.Write("New prep delay minutes (0-120, blank = no change): ");
            string delayStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(delayStr))
            {
                if (int.TryParse(delayStr, out int delay) && delay >= 0 && delay <= 120)
                    item.PrepDelayMinutes = delay;
                else
                {
                    Console.WriteLine("Error: Delay must be an integer between 0 and 120.");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(item.ItemName))
            {
                Console.WriteLine("Error: Item name cannot be empty.");
                return;
            }

            Console.WriteLine("\nItem updated successfully.");
            PrintTable(new List<MenuItem> { item });
            Pause();
        }

        // ===== Add =====
        private static void AddNewItem(List<MenuItem> items)
        {
            if (items == null) return;

            Console.WriteLine("\nAdd New Item - required fields marked *");

            // Name (required)
            Console.Write("Name*            : ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Error: Name is required.");
                return;
            }

            // Price (required, >= 0)
            Console.Write("Price*           : ");
            if (!double.TryParse(Console.ReadLine(), out double price) || price < 0)
            {
                Console.WriteLine("Error: Price must be a number >= 0.");
                return;
            }

            // Quantity (required, >= 0)
            Console.Write("Quantity*        : ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
            {
                Console.WriteLine("Error: Quantity must be an integer >= 0.");
                return;
            }

            // Description (optional)
            Console.Write("Description      : ");
            string desc = (Console.ReadLine() ?? "").Trim();

            // Availability (default true if blank)
            Console.Write("Available? (y/n) : ");
            string avail = (Console.ReadLine() ?? "").Trim().ToLower();
            bool isAvailable = (avail == "y" || avail == "yes");
            if (avail == "") isAvailable = true;

            // Prep delay (0–120, default 0 if blank)
            Console.Write("Prep delay (min, 0-120) : ");
            int delay = 0;
            string delayStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(delayStr))
            {
                if (!int.TryParse(delayStr, out delay) || delay < 0 || delay > 120)
                {
                    Console.WriteLine("Error: Delay must be an integer between 0 and 120.");
                    return;
                }
            }


            int nextId = 1;
            foreach (var it in items)
            {
                if (it.ItemId >= nextId) nextId = it.ItemId + 1;
            }

            var newItem = new MenuItem
            {
                ItemId = nextId,
                ItemName = name,
                Price = price,
                Quantity = qty,
                ItemDescription = desc,
                IsAvailable = isAvailable,
                PrepDelayMinutes = delay
            };

            items.Add(newItem);
            Console.WriteLine("\nItem added successfully.");
            PrintTable(new List<MenuItem> { newItem });
            Pause();
        }

        // ===== Delete =====
        private static void DeleteItem(List<MenuItem> items)
        {
            if (!EnsureHasItems(items)) return;

            PrintTable(items);
            Console.Write("\nEnter Item ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            // find by id without LINQ
            MenuItem item = null;
            foreach (var it in items)
            {
                if (it.ItemId == id) { item = it; break; }
            }
            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.Write("Are you sure you want to delete \"" + item.ItemName + "\"? (y/n): ");
            string confirm = (Console.ReadLine() ?? "").Trim().ToLower();
            if (confirm == "y" || confirm == "yes")
            {
                items.Remove(item);
                Console.WriteLine("Item deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
            Pause();
        }

        // ===== Helpers =====
        private static bool EnsureHasItems(List<MenuItem> items)
        {
            if (items == null || items.Count == 0)
            {
                Console.WriteLine("No menu items found.");
                return false;
            }
            return true;
        }

        private static void PrintTable(IEnumerable<MenuItem> items)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"Name",-22} {"Price",-9} {"Qty",-5} {"Avail",-7} {"Delay",-5} {"Description"}");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            foreach (var i in items)
            {
                string avail = i.IsAvailable ? "Yes" : "No";
                Console.WriteLine($"{i.ItemId,-5} {i.ItemName,-22} ${i.Price,-8:0.00} {i.Quantity,-5} {avail,-7} {i.PrepDelayMinutes,-5} {i.ItemDescription}");
            }
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            // count manually to avoid LINQ
            int count = 0; foreach (var _ in items) count++;
            Console.WriteLine("Total items: " + count);
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
