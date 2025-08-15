using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{
    public class CTLAdministratorSystem
    {
        public Administrator Admin;
        private string _currentReason;
        private List<User> _users;

        public CTLAdministratorSystem(Administrator admin, List<User> users)
        {
            Admin = admin;
            _users = users;
        }

        public List<User> requestUserList()
        {
            return new List<User>(_users); // Return a copy to prevent external modification
        }

        public string getStatus(User user)
        {
            if (user is Student student)
            {
                return student.SuspensionStatus ? "Suspended" : "Active";
            }
            return "Staff (Cannot be suspended)";
        }

        public bool inputReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return false;
            }
            _currentReason = reason;
            return true;
        }

        public bool deleteUser(User user)
        {
            if (user == null || user.UserId == Admin.UserId)
            {
                return false;
            }
            return _users.Remove(user);
        }

        public string GetCurrentReason()
        {
            return _currentReason;
        }
    }
}