using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class MenuItem
{
    public int Id { get; set; }

    public string Label { get; set; } = null!;

    public string? IconClassName { get; set; }

    public string? Href { get; set; }

    public bool? ExcludeLang { get; set; }

    public bool? IsSection { get; set; }

    public string? Target { get; set; }

    public bool? ExactMatch { get; set; }

    public string? ActiveUrl { get; set; }

    public bool? Disabled { get; set; }

    public int? ParentId { get; set; }

    public bool? IsTranslated { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<MenuItem> InverseParent { get; set; } = new List<MenuItem>();

    public virtual MenuItem? Parent { get; set; }
}
