using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class User
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public User()
        {
        }

        public User(string password, string name, string email)
        {
            Password = password;
            Name = name;
            Email = email;
        }
    }
}
