using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskCompanySection : TrackableEntity
{
    public decimal Weightage { get; set; }

    public bool IsActive { get; set; }

    public int CompanyId { get; set; }

    public int SectionId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<RiskCompanyField> RiskCompanyFields { get; set; } = new List<RiskCompanyField>();

    public virtual ICollection<RiskUserAssessmentSectionScore> RiskUserAssessmentSectionScores { get; set; } = new List<RiskUserAssessmentSectionScore>();

    public virtual RiskSection Section { get; set; } = null!;
}
