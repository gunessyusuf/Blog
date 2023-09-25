#nullable disable

using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace DataAccess;

public class User : RecordBase
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; }

    public List<Blog> Blogs { get; set; }
}
