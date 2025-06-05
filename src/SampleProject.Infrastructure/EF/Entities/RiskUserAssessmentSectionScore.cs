using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskUserAssessmentSectionScore
{
    public int Id { get; set; }

    public int AssessmentId { get; set; }

    public int CompanySectionId { get; set; }

    public int Score { get; set; }

    public int MaxPossible { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual RiskUserAssessment Assessment { get; set; } = null!;

    public virtual RiskCompanySection CompanySection { get; set; } = null!;
}
