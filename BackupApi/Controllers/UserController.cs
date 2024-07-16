using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Services;
using System.Security.Claims;
using TodosApi.Data;
using TodosApi.Middleware;
using TodosApi.Services.Redis;

namespace BackupApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IAuthUserService _authUserService;
        private ResponseHandler responseHandler = new ResponseHandler();

        public UserController(IUserServices userServices, IAuthUserService authUserService)
        {
            _userServices = userServices;
            _authUserService = authUserService;
        }

        [HttpGet("GetUser")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            var user = await _authUserService.GetUserDetail(User);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            UserDTO oUserDTO = new UserDTO();
            return responseHandler.ApiReponseHandler(oUserDTO.UserMapToDto(user));
        }
    }
}
