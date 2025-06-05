using SampleProject.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Animal
{
    public class UserBookingsRequestModel
    {
        public RegisterRequestModel User { get; set; }
        public List<UserBookingRequestModel> Bookings { get; set; }
    }
}
