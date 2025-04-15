using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class AcademyCourse : TrackableEntity
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? IsEnrolled { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
