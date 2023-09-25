#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class RoleModel : RecordBase
    {
        #region Entity'den Kopyalanan Özellikler
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        #endregion
    }
}
