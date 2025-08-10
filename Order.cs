using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public double Total { get; set; }
        public DateTime OrderTime { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public Order(int orderId, string status, double total, DateTime orderTime)
        {
            OrderId = orderId;
            Status = status;
            OrderItems = new List<OrderItem>();
            Total = total;
            OrderTime = orderTime;
        }
    }
}
