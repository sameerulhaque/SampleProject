using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskFieldValueMapping : TrackableEntity
{

    public string Text { get; set; } = null!;

    public string Value { get; set; }

    public int FieldId { get; set; }
    public virtual RiskField Field { get; set; } = null!;

    public virtual ICollection<RiskCompanyFieldCondition> RiskCompanyFieldConditions { get; set; } = new List<RiskCompanyFieldCondition>();
}
