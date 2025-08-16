using System;

namespace SWAD_assignment
{
    public class Priority : Student
    {
        public int PriorityOrderLimit { get; set; }
        public int PriorityPickUpTimeSlot { get; set; }

        public Priority(int userId, string name, string email, string password)
            : base(userId, name, email, password)
        {
            PriorityOrderLimit = 10;
            PriorityPickUpTimeSlot = 5;

            // Override base properties
            OrderLimit = PriorityOrderLimit;
            PickUpTimeSlot = PriorityPickUpTimeSlot;
        }

        public void PriorityPlaceOrder(Menu menu, FoodStall stall, Cart cart)
        {
            if (SuspensionStatus)
            {
                Console.WriteLine("You are suspended and cannot place orders.");
                return;
            }

            if (OrderLimit <= 0)
            {
                Console.WriteLine("You have reached your daily order limit.");
                return;
            }

            bool ordering = true;
            while (ordering)
            {
                Console.WriteLine();
                menu.DisplayMenu();
                Console.Write("Enter Item ID to add to cart (or 0 to checkout): ");

                if (!int.TryParse(Console.ReadLine(), out int itemId) || itemId < 0)
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (itemId == 0)
                {
                    if (cart.GetCartItems().Count == 0)
                    {
                        Console.WriteLine("Your cart is empty. Please add at least one item before checking out.");
                        continue; // go back to adding items
                    }
                    ordering = false; // Proceed to checkout
                    break;
                }

                MenuItem item = menu.GetMenuItemById(itemId);
                if (item == null)
                {
                    Console.WriteLine("Item not found.");
                    continue;
                }

                Console.Write($"Enter quantity for {item.ItemName}: ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                if (item.Quantity < qty)
                {
                    Console.WriteLine("Not enough stock available.");
                    continue;
                }

                item.ReduceStock(qty);
                cart.AddToCart(item, qty);
                Console.WriteLine($"{qty} x {item.ItemName} added to cart.");
            }

            // Checkout phase
            Console.WriteLine();
            cart.DisplayCart();

            // Change quantity step
            Console.Write("Do you want to change any quantities? (Y/N): ");
            string changeQty = Console.ReadLine()?.Trim().ToUpper();

            if (changeQty == "Y")
            {
                bool modifying = true;
                while (modifying)
                {
                    Console.Write("Enter Item ID to change quantity (or 0 to finish): ");
                    if (!int.TryParse(Console.ReadLine(), out int changeId) || changeId < 0)
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }

                    if (changeId == 0)
                    {
                        if (cart.GetCartItems().Count == 0)
                        {
                            Console.WriteLine("Your cart is empty. Returning to item selection...");
                            // Re-enter the same flow without touching Student.cs
                            PriorityPlaceOrder(menu, stall, cart);
                            return;
                        }
                        modifying = false;
                        break;
                    }

                    var existingItem = cart.GetCartItems().Find(ci => ci.Item.ItemId == changeId);
                    if (existingItem.Item == null)
                    {
                        Console.WriteLine("Item not found in cart.");
                        continue;
                    }

                    Console.Write($"Enter new quantity for {existingItem.Item.ItemName}: ");
                    if (!int.TryParse(Console.ReadLine(), out int newQty) || newQty < 0)
                    {
                        Console.WriteLine("Invalid quantity.");
                        continue;
                    }

                    if (newQty == 0)
                    {
                        existingItem.Item.IncreaseStock(existingItem.Quantity);
                        cart.UpdateQuantity(changeId, 0);
                        Console.WriteLine($"{existingItem.Item.ItemName} removed from cart.");
                    }
                    else
                    {
                        int diff = existingItem.Quantity - newQty;
                        if (diff > 0)
                        {
                            // quantity decreased => return stock
                            existingItem.Item.IncreaseStock(diff);
                        }
                        else if (diff < 0)
                        {
                            // quantity increased => need more stock
                            int increaseAmount = -diff;
                            if (existingItem.Item.Quantity < increaseAmount)
                            {
                                Console.WriteLine("Not enough stock to increase quantity.");
                                continue;
                            }
                            existingItem.Item.ReduceStock(increaseAmount);
                        }

                        cart.UpdateQuantity(changeId, newQty);
                        Console.WriteLine($"Quantity updated: {existingItem.Item.ItemName} x{newQty}");
                    }

                    cart.DisplayCart();
                }
            }

            // Final confirmation
            Console.Write("Confirm order? (Y/N): ");
            string confirm = Console.ReadLine()?.Trim().ToUpper();
            if (confirm == "Y")
            {
                // Priority users see priority slots
                stall.DisplayTimeSlotsPriority();
                Console.Write("Please select a time slot by ID: ");
                if (!int.TryParse(Console.ReadLine(), out int slotId))
                {
                    Console.WriteLine("Invalid slot selection.");
                    return;
                }

                if (stall.BookTimeSlot(slotId))
                {
                    // Booked successfully: place the order once
                    Order.PlaceOrder(cart, stall, this);
                    OrderLimit--;
                }
                else
                {
                    Console.WriteLine("Failed to book the time slot. Please try again.");
                }
            }
            else
            {
                // Order cancelled: restore stock for all cart items
                foreach (var ci in cart.GetCartItems())
                {
                    ci.Item.IncreaseStock(ci.Quantity);
                }
                Console.WriteLine("Order cancelled.");
            }
        }

        public void LastMinuteSlot(FoodStall stall)
        {
            Console.WriteLine("\n⚠️  A last minute change will result in a $2 fee. Proceed? (Y/N)");
            string confirm = Console.ReadLine()?.Trim().ToUpper();

            if (confirm != "Y")
            {
                Console.WriteLine("Understood. Returning to dashboard.");
                return;
            }

            // Display available time slots
            stall.DisplayTimeSlotsPriority();

            Console.Write("\nPlease select a new time slot by ID: ");
            if (!int.TryParse(Console.ReadLine(), out int slotId))
            {
                Console.WriteLine("Invalid slot selection.");
                return;
            }

            // Validate the selected time slot is in the last-minute window (1:30 PM - 2:15 PM)
            // Assuming slot IDs 16-18 correspond to these times as in your original code
            if (slotId < 16 || slotId > 18)
            {
                Console.WriteLine("Invalid last-minute slot selection. Only slots 16-18 are available for last-minute changes.");
                return;
            }

            // Check if the slot is actually available
            if (!stall.IsTimeSlotAvailable(slotId))
            {
                Console.WriteLine("This time slot is not available. Please choose another.");
                return;
            }

            Console.WriteLine("\nLate Change Fee: $2");
            Console.WriteLine("Would you like to proceed with this change? (Y/N)");
            confirm = Console.ReadLine()?.Trim().ToUpper();

            if (confirm == "Y")
            {
                if (stall.BookTimeSlot(slotId))
                {
                    // Get the time for display
                    var slotTime = stall.GetTimeSlotTime(slotId);

                    Console.WriteLine("\nTime slot successfully booked!");
                    Console.WriteLine($"New pick up time: {slotTime:hh:mm tt}");
                    Console.WriteLine("$2 late change fee applied");
                }
                else
                {
                    Console.WriteLine("Failed to book the time slot. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Change cancelled. Returning to dashboard.");
            }
        }
    }
}
