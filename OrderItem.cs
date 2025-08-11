using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Subtotal => Price * Quantity;

        public OrderItem(int itemId, string itemName, double price, int quantity)
        {
            ItemId = itemId;
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{ItemName} x{Quantity} - ${Subtotal:F2}";
        }
    }
}
