using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class Animal : TrackableEntity
{

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Breed { get; set; } = null!;

    public double Weight { get; set; }

    public string? Age { get; set; }

    public decimal Price { get; set; }

    public decimal PricePerShare { get; set; }

    public int TotalShares { get; set; }

    public int BookedShares { get; set; }

    public int? RemainingShares { get; set; }

    public string ImageUrl { get; set; } = null!;

    //public string? AdditionalImages { get; set; } = string.Empty;

    public string Description { get; set; } = null!;

    //public string? Features { get; set; } = string.Empty;

    public virtual ICollection<UserBooking> UserBookings { get; set; } = new List<UserBooking>();
}
