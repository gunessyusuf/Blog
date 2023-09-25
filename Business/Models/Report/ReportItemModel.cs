#nullable disable

using System.ComponentModel;

namespace Business.Models.Report
{
    public class ReportItemModel
    {
        #region Display Properties
        [DisplayName("Blog Title")]
        public string BlogTitle { get; set; }

        public string BlogContent { get; set; }

        [DisplayName("Blog Create Date")]
        public string BlogCreateDate { get; set; }

        [DisplayName("Blog Update Date")]
        public string BlogUpdateDate { get; set; }

        [DisplayName("Blog Score")]
        public double? Score { get; set; }

        [DisplayName("User")]
        public string UserName { get; set; }

        [DisplayName("User Active")]
        public string Active { get; set; }

        [DisplayName("Role")]
        public string RoleName { get; set; }

        public string Tag { get; set; }

        [DisplayName("Tag Popular")]
        public string Popular { get; set; }
        #endregion



        #region Filter Properties
        public bool? IsPopular { get; set; }

        public DateTime? BlogCreateDateInput { get; set; }
        public DateTime? BlogUpdateDateInput { get; set; }

        public int? UserId { get; set; }

        public int? RoleId { get; set; }

        public int? TagId { get; set; }
        #endregion
    }
}
