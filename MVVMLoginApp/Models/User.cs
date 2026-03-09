using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLoginApp.Models
{
    // The Model is a simple data container.
    // It knows nothing about the UI.
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
