using Business.Models.Report;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Areas.Report.Models;

namespace MVC.Areas.Report.Controllers
{
    [Area("Report")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ITagService _tagService;

        public HomeController(IReportService reportService, IUserService userService, IRoleService roleService, ITagService tagService)
        {
            _reportService = reportService;
            _userService = userService;
            _roleService = roleService;
            _tagService = tagService;
        }

        // GET: HomeController
        public ActionResult Index()
        {
            var model = _reportService.GetList(false);
            var viewModel = new HomeIndexViewModel()
            {
                Report = model,
                UserSelectList = new SelectList(_userService.GetListByRole(), "Id", "UserName"),
                RoleSelectList = new SelectList(_roleService.Query().ToList(), "Id", "Name"),
                TagMultiSelectlist = new MultiSelectList(_tagService.GetList(), "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        //public IActionResult Index(HomeIndexViewModel viewModel)
        public IActionResult Index(FilterModel filter)
        {
            var model = _reportService.GetList(false, filter);
            return PartialView("_Report", model);
        }

        [HttpPost]
        public IActionResult IndexGroupBy()
        {
            var model = _reportService.GetListGroupBy();
            return PartialView("_ReportGroupBy", model);
        }
    }
}
