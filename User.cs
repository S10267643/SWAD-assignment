using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public abstract class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        protected User(int userId, string name, string email, string password)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Password = password;
        }

        public static bool IsEmailUnique(List<User> users, string email)
        {
            return !users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public static bool ValidateCredentials(string email, string password)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Contains("@") &&
                   !string.IsNullOrWhiteSpace(password) &&
                   password.Length >= 3;
        }
    }
}