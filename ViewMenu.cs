// ViewMenu.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public static class ViewMenu
    {
        public static void Show(FoodStallStaff staff)
        {
            if (staff == null)
            {
                Console.WriteLine("Error: No staff user provided.");
                return;
            }

            if (staff.Stall == null)
            {
                Console.WriteLine("You are not assigned to any stall yet.");
                return;
            }

            if (staff.Stall.Menu == null)
            {
                staff.Stall.Menu = new Menu();
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("==============================================");
                Console.WriteLine(" " + staff.Stall.StallName + " - Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine("1) List all items");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                if (choice == "0") break;

                if (choice == "1")
                {
                    ListAll(staff.Stall.Menu.MenuItems);
                    Pause();
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private static void ListAll(List<MenuItem> items)
        {
            if (items == null || items.Count == 0)
            {
                Console.WriteLine("No menu items found.");
                return;
            }

            var result = items.OrderBy(i => i.ItemId);
            PrintTable(result);
        }

        private static void PrintTable(IEnumerable<MenuItem> items)
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"Name",-22} {"Price",-9} {"Qty",-5} {"Avail",-7} {"Delay",-5} {"Description"}");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");

            foreach (var i in items)
            {
                string avail = i.IsAvailable ? "Yes" : "No";
                Console.WriteLine($"{i.ItemId,-5} {i.ItemName,-22} ${i.Price,-8:0.00} {i.Quantity,-5} {avail,-7} {i.PrepDelayMinutes,-5} {i.ItemDescription}");
            }

            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Total items: {items.Count()}");
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
