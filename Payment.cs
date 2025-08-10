using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; }

        public Payment()
        {
        }

        public Payment(int paymentId, string paymentMethod)
        {
            PaymentId = paymentId;
            PaymentMethod = paymentMethod;
        }
    }
}
