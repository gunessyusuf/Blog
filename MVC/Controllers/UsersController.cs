using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
	public class UsersController : Controller
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		public IActionResult GetUsersByRole(int roleId)
		{
			var users = _userService.GetListByRole(roleId);
			return Json(users);
		}
	}
}
