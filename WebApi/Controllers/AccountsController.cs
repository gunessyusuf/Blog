using Business.Models;
using Business.Models.Account;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Utils.Bases;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly JwtUtilBase _jwtUtil;

        public AccountsController(IAccountService accountService, JwtUtilBase jwtUtil)
        {
            _accountService = accountService;
            _jwtUtil = jwtUtil;
        }

        [HttpPost("Login")]
        public IActionResult Login(AccountLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var userResult = new UserModel();
                var result = _accountService.Login(model, userResult);
                if (result.IsSuccessful)
                {
                    var jwt = _jwtUtil.CreateJwt(userResult.UserName, userResult.Role.Name, userResult.Id.ToString());
                    return Ok(jwt);
                }
                ModelState.AddModelError("Message", result.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
