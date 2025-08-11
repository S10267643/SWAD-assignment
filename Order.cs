using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Order
    {
        private static int orderCounter = 1;
        public int OrderId { get; set; }
        public List<(MenuItem Item, int Quantity)> Items { get; set; }
        public double TotalPrice { get; set; }
        public string QRCode { get; set; }

        public Order(List<(MenuItem Item, int Quantity)> items, double totalPrice)
        {
            OrderId = orderCounter++;
            Items = items;
            TotalPrice = totalPrice;
        }

        public void GenerateQRCode()
        {
            QRCode = $"QR-{OrderId}-{DateTime.Now.Ticks}";
        }

        public void NotifyStall(FoodStall stall)
        {
            stall.NotifyNewOrder(this);
        }

        public static Order PlaceOrder(Cart cart, FoodStall stall)
        {
            var order = new Order(cart.GetCartItems(), cart.CalculateTotalPrice());
            cart.EmptyCart();
            order.GenerateQRCode();
            order.NotifyStall(stall);
            Console.WriteLine($"Order confirmed: {order.OrderId} | QR Code: {order.QRCode}");
            return order;
        }
    }
}
