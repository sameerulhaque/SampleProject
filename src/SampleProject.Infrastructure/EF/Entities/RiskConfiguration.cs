using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskConfiguration
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Version { get; set; } = null!;

    public int CompanyId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<RiskUserAssessment> RiskUserAssessments { get; set; } = new List<RiskUserAssessment>();
}
