using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class MenuItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; } = true;   
        public int PrepDelayMinutes { get; set; } = 0;  
        public MenuItem() { }

        public MenuItem(int itemId, string itemName, double price, string itemDescription, int quantity)
        {
            ItemId = itemId;
            ItemName = itemName;
            Price = price;
            ItemDescription = itemDescription;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"ID: {ItemId} | {ItemName} - ${Price:F2} | {ItemDescription} | Stock: {Quantity}";
        }

        public bool ReduceStock(int amount)
        {
            if (Quantity >= amount)
            {
                Quantity -= amount;
                return true;
            }
            return false; // Not enough stock
        }

        public void IncreaseStock(int amount)
        {
            Quantity += amount;
        }
    }
}

