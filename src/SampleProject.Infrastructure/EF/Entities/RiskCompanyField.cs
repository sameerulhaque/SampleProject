using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskCompanyField : TrackableEntity
{
    public int CompanySectionId { get; set; }

    public bool IsActive { get; set; }

    public int FieldId { get; set; }

    public int? MaxScore { get; set; }

    public virtual ICollection<AssessmentAnswer> AssessmentAnswers { get; set; } = new List<AssessmentAnswer>();

    public virtual RiskCompanySection CompanySection { get; set; } = null!;

    public virtual RiskField Field { get; set; } = null!;

    public virtual ICollection<RiskCompanyFieldCondition> RiskCompanyFieldConditions { get; set; } = new List<RiskCompanyFieldCondition>();
}
