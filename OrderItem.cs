using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class OrderItem
    {
        public int ItemQuantity { get; set; }
        public double Subtotal { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(int itemQuantity, double subtotal)
        {
            ItemQuantity = itemQuantity;
            Subtotal = subtotal;
        }
    }
}
