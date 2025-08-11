using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Cart
    {
        private Dictionary<int, (MenuItem Item, int Quantity)> cartItems = new Dictionary<int, (MenuItem, int)>();

        public void AddToCart(MenuItem item, int quantity)
        {
            if (cartItems.ContainsKey(item.ItemId))
                cartItems[item.ItemId] = (item, cartItems[item.ItemId].Quantity + quantity);
            else
                cartItems[item.ItemId] = (item, quantity);
        }

        public void UpdateQuantity(int itemId, int quantity)
        {
            if (cartItems.ContainsKey(itemId))
            {
                if (quantity <= 0)
                {
                    cartItems.Remove(itemId);
                }
                else
                {
                    var existing = cartItems[itemId];
                    cartItems[itemId] = (existing.Item, quantity);
                }
            }
        }

        public List<(MenuItem Item, int Quantity)> GetCartItems()
        {
            var list = new List<(MenuItem, int)>();
            foreach (var kv in cartItems)
            {
                list.Add((kv.Value.Item, kv.Value.Quantity));
            }
            return list;
        }

        public double CalculateTotalPrice()
        {
            double total = 0;
            foreach (var kv in cartItems)
            {
                total += kv.Value.Item.Price * kv.Value.Quantity;
            }
            return total;
        }

        public void EmptyCart()
        {
            cartItems.Clear();
        }

        public void DisplayCart()
        {
            Console.WriteLine("=== Your Cart ===");
            foreach (var kv in cartItems)
            {
                Console.WriteLine($"{kv.Value.Item.ItemName} x{kv.Value.Quantity} - ${kv.Value.Item.Price * kv.Value.Quantity:F2}");
            }
            Console.WriteLine($"Total: ${CalculateTotalPrice():F2}");
        }
    }
}
