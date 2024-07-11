using Model;
using Model.Services;
using System.Security.Claims;

namespace TodosApi.Services.Redis
{
    public interface IAuthUserService
    {
        public Task<User> GetUserDetail(ClaimsPrincipal user);
    }

    public class AuthUserService : IAuthUserService
    {
        private readonly IUserServices _userServices;

        public AuthUserService(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task<User> GetUserDetail(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return null;
            }

            User userDetail = await _userServices.GetUserById(Convert.ToInt32(userId));
            if (userDetail == null)
            {
                return null;
            }

            return userDetail;
        }
    }
}
