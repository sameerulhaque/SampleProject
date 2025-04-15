using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class UserPermission
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PermissionId { get; set; }

    public DateTime? GrantedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Permission? Permission { get; set; }

    public virtual User? User { get; set; }
}
