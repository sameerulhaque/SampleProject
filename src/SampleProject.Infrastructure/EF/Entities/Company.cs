using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class Company : TrackableEntity
{

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<RiskCompanySection> RiskCompanySections { get; set; } = new List<RiskCompanySection>();

    public virtual ICollection<RiskConfiguration> RiskConfigurations { get; set; } = new List<RiskConfiguration>();
}
