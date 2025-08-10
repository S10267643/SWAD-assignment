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



        public MenuItem(int itemId, string itemName, double price,
                        string itemDescription, int quantity)
        {
            ItemId = itemId;
            ItemName = itemName;
            Price = price;
            ItemDescription = itemDescription;
            Quantity = quantity;
        }
    }
}
