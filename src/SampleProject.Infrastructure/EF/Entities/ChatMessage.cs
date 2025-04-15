using System;
using System.Collections.Generic;

namespace SampleProject.Infrastructure.EF.Entities;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int? RoomId { get; set; }

    public int? UserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ChatRoom? Room { get; set; }

    public virtual User? User { get; set; }
}
