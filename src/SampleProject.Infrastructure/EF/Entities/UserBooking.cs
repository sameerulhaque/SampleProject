using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class UserBooking : TrackableEntity
{
    public int UserId { get; set; }

    public int AnimalId { get; set; }

    public string AnimalName { get; set; } = null!;

    public int Shares { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Animal Animal { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
