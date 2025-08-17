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
            DateTime startTime = DateTime.Today.AddHours(10);
            DateTime endTime = DateTime.Today.AddHours(18);
            int slotId = 1;
            DateTime now = DateTime.Now;

            while (startTime < endTime)
            {
                bool isAvailable = startTime >= now.AddMinutes(30) && _random.Next(0, 5) > 0;
                _timeSlots.Add(slotId, (startTime, isAvailable));
                startTime = startTime.AddMinutes(15);
                slotId++;
            }
        }

        public void DisplayTimeSlots()
        {
            Console.WriteLine("\n   FOOD STALL AVAILABLE TIME SLOTS   ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine($"Current time: {DateTime.Now.ToString("hh:mm tt")}");
            Console.WriteLine($"Earliest available pickup: {DateTime.Now.AddMinutes(30).ToString("hh:mm tt")}");
            Console.WriteLine();

            int slotsPerRow = 4;
            int currentSlot = 0;
            DateTime minimumTime = DateTime.Now.AddMinutes(30);

            foreach (var slot in _timeSlots)
            {
                string displayText;
                bool isAvailable = slot.Value.IsAvailable && slot.Value.Time >= minimumTime;

                if (slot.Value.Time < minimumTime)
                {
                    displayText = $"{slot.Key}. [TOO SOON]";
                }
                else if (!isAvailable)
                {
                    displayText = $"{slot.Key}. [UNAVAILABLE]";
                }
                else
                {
                    displayText = $"{slot.Key}. {slot.Value.Time.ToString("hh:mm tt")}";
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
        }

        public bool BookTimeSlot(int slotId, bool isPriority = false)
        {
            if (!_timeSlots.ContainsKey(slotId))
            {
                Console.WriteLine("Invalid slot ID.");
                return false;
            }

            var slot = _timeSlots[slotId];
            DateTime minimumTime = DateTime.Now.AddMinutes(30);

            if (!isPriority)
            {
                if (slot.Time < minimumTime)
                {
                    Console.WriteLine("This time slot is too soon. Please choose a slot at least 30 minutes from now.");
                    return false;
                }

                if (!slot.IsAvailable)
                {
                    Console.WriteLine("This time slot is not available.");
                    return false;
                }
            }

            Console.Write($"Do you confirm that you want time slot {slotId}? (Y/N): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("Booking cancelled.");
                return false;
            }

            // Mark slot as unavailable after booking
            _timeSlots[slotId] = (slot.Time, false);
            string qrCode = $"QR-{DateTime.Now.Ticks.ToString().Substring(10)}";

            Console.WriteLine("\nTime slot successfully booked!");
            Console.WriteLine($"QR Code: {qrCode}");
            Console.WriteLine($"Pickup time: {slot.Time.ToString("hh:mm tt")}");

            return true;
        }

        public bool ChangeTimeSlot(int currentSlotId)
        {
            Console.WriteLine("\nChanging your time slot will incur a $2 fee. Continue? (Y/N)");
            if (Console.ReadLine()?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("Time slot change cancelled.");
                return false;
            }

            DisplayTimeSlots();
            Console.Write("\nEnter new time slot ID: ");
            if (!int.TryParse(Console.ReadLine(), out int newSlotId) || !_timeSlots.ContainsKey(newSlotId))
            {
                Console.WriteLine("Invalid slot ID.");
                return false;
            }

            var newSlot = _timeSlots[newSlotId];
            if (!newSlot.IsAvailable)
            {
                Console.WriteLine("This time slot is not available.");
                return false;
            }

            Console.Write($"Do you confirm that you want time slot {newSlotId}? You will be charged $2. (Y/N): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("Time slot change cancelled.");
                return false;
            }

            // Release old slot and book new one
            _timeSlots[currentSlotId] = (_timeSlots[currentSlotId].Time, true);
            _timeSlots[newSlotId] = (newSlot.Time, false);

            string newQrCode = $"QR-{DateTime.Now.Ticks.ToString().Substring(10)}";

            Console.WriteLine("\nTime slot successfully changed!");
            Console.WriteLine($"New QR Code: {newQrCode}");
            Console.WriteLine($"New pickup time: {newSlot.Time.ToString("hh:mm tt")}");
            Console.WriteLine("$2 late change fee applied");

            return true;
        }

        // Helper method to suggest alternative slots
        private void SuggestAlternativeSlots(int originalSlotId)
        {
            Console.WriteLine("\nSuggested alternative slots:");
            int suggestions = 0;
            DateTime minimumTime = DateTime.Now.AddMinutes(30);

            foreach (var slot in _timeSlots)
            {
                if (slot.Key != originalSlotId &&
                    slot.Value.IsAvailable &&
                    slot.Value.Time >= minimumTime &&
                    suggestions < 3)
                {
                    Console.WriteLine($"{slot.Key}. {slot.Value.Time.ToString("hh:mm tt")}");
                    suggestions++;
                }
            }

            if (suggestions == 0)
            {
                Console.WriteLine("No alternative slots available. Please try again later.");
            }
        }
        public void DisplayTimeSlotsPriority()
        {
            Console.WriteLine("\n   FOOD STALL AVAILABLE TIME SLOTS   ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine($"Current time: {DateTime.Now.ToString("hh:mm tt")}");
            Console.WriteLine($"Earliest available pickup: {DateTime.Now.AddMinutes(30).ToString("hh:mm tt")}");
            Console.WriteLine();

            int slotsPerRow = 4;
            int currentSlot = 0;
            DateTime minimumTime = DateTime.Now.AddMinutes(30);

            foreach (var slot in _timeSlots)
            {
                string displayText;
                bool isAvailable = slot.Value.IsAvailable && slot.Value.Time >= minimumTime;

                if (slot.Value.Time < minimumTime)
                {
                    displayText = $"{slot.Key}. [TOO SOON]";
                }
                else if (!isAvailable)
                {
                    displayText = $"{slot.Key}. [UNAVAILABLE]";
                }
                else
                {
                    displayText = $"{slot.Key}. {slot.Value.Time.ToString("hh:mm tt")}";
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

