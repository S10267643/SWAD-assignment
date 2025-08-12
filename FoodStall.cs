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
        private Dictionary<int, (DateTime Time, bool IsAvailable)> _timeSlots;
        private Random _random;

        public FoodStall()
        {
            Menu = new Menu();
            Orders = new List<Order>();
            _random = new Random();
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

        private void InitializeTimeSlots()
        {
            _timeSlots = new Dictionary<int, (DateTime, bool)>();
            DateTime startTime = DateTime.Today.AddHours(10); // 10:00 AM
            DateTime endTime = DateTime.Today.AddHours(18);   // 6:00 PM
            int slotId = 1;

            while (startTime < endTime)
            {
                // Randomly mark some slots as unavailable (about 20% chance)
                bool isAvailable = _random.Next(0, 5) > 0; // 80% chance of being available
                _timeSlots.Add(slotId, (startTime, isAvailable));

                startTime = startTime.AddMinutes(15);
                slotId++;
            }
        }

        public void DisplayTimeSlots()
        {
            Console.WriteLine("\n   FOOD STALL AVAILABLE TIME SLOTS   ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine();

            int slotsPerRow = 4;
            int currentSlot = 0;

            foreach (var slot in _timeSlots)
            {
                string displayText = $"{slot.Key}. {slot.Value.Time.ToString("hh:mm tt")}";

                // Mark unavailable slots
                if (!slot.Value.IsAvailable)
                {
                    displayText = $"{slot.Key}. [UNAVAILABLE]";
                }

                Console.Write(displayText.PadRight(25));

                currentSlot++;
                if (currentSlot % slotsPerRow == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n══════════════════════════════════════");
            Console.WriteLine($"Total available slots: {_timeSlots.Count(s => s.Value.IsAvailable)}\n");
        }

        public void DisplayTimeSlotsPriority()
        {
            Console.WriteLine("\n   PRIORITY TIME SLOTS   ");
            Console.WriteLine("════════════════════════════");
            Console.WriteLine();

            // Priority students see all slots as available
            foreach (var slot in _timeSlots)
            {
                Console.WriteLine($"{slot.Key}. {slot.Value.Time.ToString("hh:mm tt")} (Priority)");
            }

            Console.WriteLine("\n════════════════════════════");
        }

        public bool BookTimeSlot(int slotId, bool isPriority = false)
        {
            if (!_timeSlots.ContainsKey(slotId))
            {
                Console.WriteLine("Invalid slot ID.");
                return false;
            }

            var slot = _timeSlots[slotId];

            if (!isPriority && !slot.IsAvailable)
            {
                Console.WriteLine("This time slot is not available.");
                return false;
            }

            // Mark as booked (unavailable for others)
            if (!isPriority)
            {
                _timeSlots[slotId] = (slot.Time, false);
            }

            Console.WriteLine($"Time slot booked: {slot.Time.ToString("hh:mm tt")}");
            return true;
        }
        public bool IsTimeSlotAvailable(int slotId)
        {
            if (_timeSlots.TryGetValue(slotId, out var slot))
            {
                return slot.IsAvailable;
            }
            return false;
        }

        public DateTime GetTimeSlotTime(int slotId)
        {
            if (_timeSlots.TryGetValue(slotId, out var slot))
            {
                return slot.Time;
            }
            throw new ArgumentException("Invalid slot ID");
        }
    }
}

