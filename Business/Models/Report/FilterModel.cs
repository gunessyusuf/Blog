#nullable disable

using System.ComponentModel;

namespace Business.Models.Report
{
    public class FilterModel
    {
        [DisplayName("Blog Title")]
        public string BlogTitle { get; set; }

        [DisplayName("Create Date")]
        public DateTime? CreateDateBegin { get; set; }

        public DateTime? CreateDateEnd { get; set; }

        [DisplayName("Score")]
        public double? ScoreBegin { get; set; }

        public double? ScoreEnd { get; set; }

        [DisplayName("User")]
        public int? UserId { get; set; }

		[DisplayName("Role")]
		public int? RoleId { get; set; }

        [DisplayName("Tags")]
        public List<int> TagIds { get; set; }
    }
}
