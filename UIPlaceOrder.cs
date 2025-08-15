using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class UIPlaceOrder
    {
        private CTLPlaceOrder _controller;

        public UIPlaceOrder(CTLPlaceOrder controller)
        {
            _controller = controller;
        }

        public void StartOrderProcess(FoodStall stall)
        {
            bool continueOrdering = true;
            while (continueOrdering)
            {
                DisplayMenu();
                var (itemId, quantity) = GetItemSelection();

                if (itemId == 0) // Checkout selected
                {
                    if (_controller.GetCart().GetCartItems().Count == 0)
                    {
                        Console.WriteLine("Your cart is empty!");
                        continue;
                    }
                    continueOrdering = !ProcessCheckout(stall);
                }
                else
                {
                    if (!_controller.AddToCart(itemId, quantity))
                    {
                        Console.WriteLine("Failed to add item. Check availability.");
                    }
                    DisplayCart();
                }
            }
        }

        private (int, int) GetItemSelection()
        {
            Console.Write("Enter Item ID (0 to checkout): ");
            int itemId = int.Parse(Console.ReadLine());

            if (itemId == 0) return (0, 0);

            Console.Write("Enter quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            return (itemId, quantity);
        }

        private bool ProcessCheckout(FoodStall stall)
        {
            DisplayCart();
            Console.Write("Confirm order? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                var order = _controller.Checkout(stall);
                Console.WriteLine($"Order confirmed! QR: {order.QRCode}");
                return true;
            }
            return false;
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\n=== MENU ===");
            foreach (var item in _controller.GetMenu().GetAllItems())
            {
                Console.WriteLine($"{item.ItemId}. {item.ItemName} - ${item.Price} (Stock: {item.Quantity})");
            }
        }

        private void DisplayCart()
        {
            Console.WriteLine("\n=== YOUR CART ===");
            foreach (var item in _controller.GetCart().GetCartItems())
            {
                Console.WriteLine($"{item.Item.ItemName} x{item.Quantity}");
            }
            Console.WriteLine($"TOTAL: ${_controller.GetCart().CalculateTotalPrice()}");
        }
    }
}
