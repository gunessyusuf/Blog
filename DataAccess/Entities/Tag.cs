#nullable disable

using System.ComponentModel.DataAnnotations;
using AppCore.Records.Bases;

namespace DataAccess;

public class Tag : RecordBase
{
    [Required]
    [StringLength(75)]
    public string Name { get; set; }

    public bool IsPopular { get; set; }

    public List<BlogTag> BlogTags { get; set; }
}
