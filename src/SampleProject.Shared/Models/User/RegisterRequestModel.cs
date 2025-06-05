using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.User
{
    public class RegisterRequestModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; } = "User";
    }
}
