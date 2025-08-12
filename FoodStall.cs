using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SWAD_assignment
{
    public class FoodStall
    {
        public int StallId { get; set; }
        public string StallName { get; set; }
        public string Description { get; set; }
        public int OperatingHours { get; set; }
        public string Location { get; set; }
        public Menu Menu { get; set; }
        public List<Order> Orders { get; set; }

        public FoodStall()
        {
            Menu = new Menu();
            Orders = new List<Order>();
        }

        public void NotifyNewOrder(Order order)
        {
            Orders.Add(order);
            Console.WriteLine($"New order received at {StallName}: #{order.OrderId}");
        }

        public void DisplayAllOrders()
        {
            if (Orders.Count == 0)
            {
                Console.WriteLine("No orders have been placed yet.");
                return;
            }

            Console.WriteLine($"\n=== Orders for {StallName} ===");
            foreach (var order in Orders)
            {
                Console.WriteLine($"\nOrder ID: {order.OrderId}");
                Console.WriteLine($"Customer: {order.OrderedBy.Name}");
                Console.WriteLine($"Total: {order.TotalPrice:C}");
                Console.WriteLine("Items:");
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"- {item.Item.ItemName} x{item.Quantity} @ {item.Item.Price:C}");
                }
            }
        }
        public void CancelOrder(int orderId)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine($"Order #{orderId} not found.");
                return;
            }

            // Get cancellation reason
            string reason = GetCancellationReason();
            if (reason == null) return; // Cancellation aborted

            // Restore stock
            foreach (var item in order.Items)
            {
                item.Item.IncreaseStock(item.Quantity);
            }

            // Notify student with reason
            order.OrderedBy.Notifications.Add(
                $"Your order #{orderId} from {StallName} has been cancelled. Reason: {reason}");

            Orders.Remove(order);
            Console.WriteLine($"Order #{orderId} has been cancelled.");
        }

        private string GetCancellationReason()
        {
            var reasons = new Dictionary<int, string>
        {
            {1, "Item out of stock"},
            {2, "Stall closed unexpectedly"},
            {3, "Payment issue"},
            {4, "Other reason"}
        };

            Console.WriteLine("\nSelect cancellation reason:");
            foreach (var reason in reasons)
            {
                Console.WriteLine($"{reason.Key}. {reason.Value}");
            }

            Console.Write("Enter reason number (or 0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
            {
                Console.WriteLine("Cancellation aborted.");
                return null;
            }

            if (reasons.TryGetValue(choice, out string selectedReason))
            {
                if (choice == 4) // Other reason
                {
                    Console.Write("Enter specific reason: ");
                    selectedReason = Console.ReadLine();
                }
                return selectedReason;
            }

            Console.WriteLine("Invalid reason selected. Using default reason.");
            return "Unspecified reason";
        }
    }
}

