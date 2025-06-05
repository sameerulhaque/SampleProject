using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class RiskField : TrackableEntity
{
    public string? EndpointURL { get; set; }

    public string? Label { get; set; }

    public int SectionId { get; set; }

    public string? FieldType_ { get; set; }

    public bool? IsRequired_ { get; set; }

    public string? Placeholder_ { get; set; }

    public int? OrderIndex_ { get; set; }

    public virtual RiskSection Section { get; set; } = null!;
    public virtual ICollection<RiskFieldValueMapping> RiskFieldValueMappings { get; set; } = new List<RiskFieldValueMapping>();
    public virtual ICollection<RiskCompanyField> RiskCompanyFields { get; set; } = new List<RiskCompanyField>();

}
