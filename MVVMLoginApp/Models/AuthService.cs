using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLoginApp.Models
{
    public class AuthService
    {
        private readonly List<User> _users = new()
        {
            new User
            {
                Username = "admin",
                Password = "password123",
                DisplayName = "Admin User"
            },
            new User
            {
                Username = "student",
                Password = "learn2024",
                DisplayName = "Student User"
            }
        };

        public User? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);
        }
    }
}
