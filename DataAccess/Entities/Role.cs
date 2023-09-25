#nullable disable

using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace DataAccess;

public class Role : RecordBase
{
    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}
