namespace SWAD_assignment
{
    public class Student : User
    {
        public int StudentId { get; set; }
        public bool SuspensionStatus { get; set; }
        public int OrderLimit { get; set; }
        public int PickUpTimeSlot { get; set; }

        public Student(int userId, string name, string email, string password)
             : base(userId, name, email, password)
        {
            StudentId = userId;
            SuspensionStatus = false;
            OrderLimit = 5;          // Default order limit
            PickUpTimeSlot = 1;
        }
        public void SendFeedback(List<FoodStallStaff> stalls)
        {
            if (SuspensionStatus)
            {
                Console.WriteLine("Your account is suspended and cannot send feedback.");
                return;
            }

            Console.WriteLine("\nSelect a stall to send feedback to:");
            for (int i = 0; i < stalls.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {stalls[i].Stall.StallName}");
            }

            if (!int.TryParse(Console.ReadLine(), out int stallChoice) || stallChoice < 1 || stallChoice > stalls.Count)
            {
                Console.WriteLine("Invalid stall selection.");
                return;
            }

            Console.Write("Enter your feedback: ");
            string description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Feedback cannot be empty.");
                return;
            }

            var feedback = new Feedback
            {
                FeedbackId = Program.GetNextFeedbackId(),
                Description = description,
                FromStudent = this
            };

            stalls[stallChoice - 1].ReceiveFeedback(feedback);
            Console.WriteLine("Feedback sent successfully!");
        }

        public void PlaceOrder(Menu menu, FoodStall stall, Cart cart)
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
                            PlaceOrder(menu, stall, cart);
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
                            existingItem.Item.IncreaseStock(diff);
                        }
                        else if (diff < 0)
                        {
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
                // Proceed with order: stock was already reduced, so just place order
                Order.PlaceOrder(cart, stall);
                OrderLimit--;
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
    }
}