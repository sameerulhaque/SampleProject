using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskCompanyFieldCondition
{
    public int Id { get; set; }

    public int CompanyFieldId { get; set; }

    public int FieldValueMappingId { get; set; }

    public string? Operator { get; set; }

    public string? Value { get; set; }

    public string? ValueTo { get; set; }

    public int RiskScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsActive { get; set; }

    public virtual RiskCompanyField CompanyField { get; set; } = null!;

    public virtual RiskFieldValueMapping FieldValueMapping { get; set; } = null!;
}
