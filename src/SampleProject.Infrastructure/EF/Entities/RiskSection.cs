using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskSection : TrackableEntity
{
    public string SectionName { get; set; } = null!;

    public virtual ICollection<RiskCompanySection> RiskCompanySections { get; set; } = new List<RiskCompanySection>();

    public virtual ICollection<RiskField> RiskFields { get; set; } = new List<RiskField>();
}
