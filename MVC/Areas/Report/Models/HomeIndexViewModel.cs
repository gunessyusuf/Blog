using Business.Models.Report;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Areas.Report.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ReportItemModel> Report { get; set; }
        public FilterModel Filter { get; set; }
        public SelectList UserSelectList { get; set; }
        public SelectList RoleSelectList { get; set; }
        public MultiSelectList TagMultiSelectlist { get; set; }

        public IEnumerable<ReportItemGroupByModel> ReportGroupBy { get; set; }
    }
}
