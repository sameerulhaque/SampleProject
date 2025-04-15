using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class Invoice : TrackableEntity
{

    public decimal Amount { get; set; }

    public string ClientName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DueDate { get; set; }
}
