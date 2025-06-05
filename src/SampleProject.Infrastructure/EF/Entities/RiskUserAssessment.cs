using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskUserAssessment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RiskConfigurationId { get; set; }

    public int TotalScore { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<AssessmentAnswer> AssessmentAnswers { get; set; } = new List<AssessmentAnswer>();

    public virtual RiskConfiguration RiskConfiguration { get; set; } = null!;

    public virtual ICollection<RiskUserAssessmentSectionScore> RiskUserAssessmentSectionScores { get; set; } = new List<RiskUserAssessmentSectionScore>();

    public virtual User User { get; set; } = null!;
}
