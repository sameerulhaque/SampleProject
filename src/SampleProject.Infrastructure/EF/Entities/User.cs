using SampleProject.Infrastructure.EF.Models;
using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class User : TrackableEntity
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; } = null!;

    public string? Role { get; set; } = null!;

    public DateTime? LastAccessed { get; set; }

    public string? Phone { get; set; }
    public string? Address { get; set; }


    public virtual ICollection<RiskUserAssessment> RiskUserAssessments { get; set; } = new List<RiskUserAssessment>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<UserBooking> UserBookings { get; set; } = new List<UserBooking>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
