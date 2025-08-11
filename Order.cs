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
        public int StudentId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public double Total => OrderItems.Sum(item => item.Subtotal);
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }

        public Order(int orderId, int studentId)
        {
            OrderId = orderId;
            StudentId = studentId;
            OrderItems = new List<OrderItem>();
            OrderTime = DateTime.Now;
            Status = "Pending";
        }
    }
}
