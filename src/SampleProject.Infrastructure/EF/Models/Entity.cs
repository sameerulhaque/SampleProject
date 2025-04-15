using System.ComponentModel.DataAnnotations;

namespace SampleProject.Infrastructure.EF.Models;

public abstract class Entity
{
    [Key]
    public int Id { get; set; }
    public bool? IsDeleted { get; set; } = false;

}
