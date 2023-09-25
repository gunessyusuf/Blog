#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppCore.Records.Bases;

namespace DataAccess;

public class Blog : RecordBase
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; }

    [Required]
    [StringLength(300)]
    public string Content { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public double? Score { get; set; }
    
    public int UserId { get; set; }

    public User User { get; set; }

    public List<BlogTag> BlogTags { get; set; }



    #region Binary Data
    [Column(TypeName = "image")]
    public byte[] Image { get; set; }

    [StringLength(5)]
    public string ImageExtension { get; set; } // .jpg, .png
    #endregion
}
