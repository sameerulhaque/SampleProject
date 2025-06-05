using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.User
{
    public class LoginResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? LastAccessed { get; set; }
        public string? Phone { get; set; }
        public string Token { get; set; }
        public int Shares { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }
}
