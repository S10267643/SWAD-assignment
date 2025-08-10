using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Administrator : User
    {
        public int AdminId { get; set; }

        public Administrator()
        {
        }

        public Administrator(string password, string name, string email, int adminId)
            : base(password, name, email)
        {
            AdminId = adminId;
        }
    }
}
