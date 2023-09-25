#nullable disable

using System.ComponentModel;

namespace Business.Models.Report
{
    public class ReportItemGroupByModel
    {
        public int TagId { get; set; }

        [DisplayName("Tag")]
        public string TagName { get; set; }

        [DisplayName("Average Score")]
        public string AverageScore { get; set; }
    }
}
