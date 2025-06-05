using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Animal
{
    public class UserBookingRequestModel
    {
        public int? UserId { get; set; }

        public int AnimalId { get; set; }

        public string AnimalName { get; set; } = null!;

        public int Shares { get; set; }
    }

    public class UserBookingResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int AnimalId { get; set; }

        public string AnimalName { get; set; } = null!;

        public int Shares { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = null!;
        public string UserName { get; set; }
    }

}
