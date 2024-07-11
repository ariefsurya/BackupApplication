using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO.Compression;
using RabbitMqProductApi.RabbitMQ;
using Model;
using Model.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodosApi.Data;
using System.Transactions;

namespace BackupApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserServices _userServices;
        private readonly ICompanyServices _companyServices;
        private ResponseHandler responseHandler = new ResponseHandler();

        public AuthenticationController(IConfiguration configuration, IUserServices userServices, ICompanyServices companyServices)
        {
            _configuration = configuration;
            _userServices = userServices;
            _companyServices = companyServices;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                // Validate the user credentials (this is just an example)
                if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                {
                    throw new BadHttpRequestException("Email or Password cannot be empty.");
                }
                User oUser = await _userServices.GetUser(login.Email, login.Password);
                if (oUser == null)
                {
                    throw new BadHttpRequestException("Email or Password is incorrect.");
                }

                var token = GenerateJwtToken(oUser.Id.ToString(), oUser.Email);
                oUser.Token = token;
                oUser.LastLogin = DateTime.UtcNow;
                User user = await _userServices.UpdateUser(oUser);

                UserDTO userDTO = new UserDTO();
                return responseHandler.ApiReponseHandler(userDTO.UserMapToDto(user));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO oRegisterDTO)
        {
            try
            {
                //validation
                if (string.IsNullOrEmpty(oRegisterDTO.CompanyName))
                {
                    throw new BadHttpRequestException("Company Name must be filled.");
                }
                if (oRegisterDTO == null || string.IsNullOrEmpty(oRegisterDTO.Email) || string.IsNullOrEmpty(oRegisterDTO.Password))
                {
                    throw new BadHttpRequestException("Username And Password cannot be empty.");
                }
                bool isUserAlreadyExists = await _userServices.IsEmailExists(oRegisterDTO.Email);
                if (isUserAlreadyExists)
                {
                    throw new BadHttpRequestException("Email Already Exists.");
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Company oCompany = await _companyServices.AddCompany(new Company
                    {
                        CompanyName = oRegisterDTO.CompanyName,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                    User oUser = await _userServices.AddUser(new User
                    {
                        Username = oRegisterDTO.Username,
                        Email = oRegisterDTO.Email,
                        Password = oRegisterDTO.Password,
                        PhoneNumber = oRegisterDTO.PhoneNumber,
                        CompanyId = oCompany.Id,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                    scope.Complete();
                    return responseHandler.ApiReponseHandler("Register Success");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private string GenerateJwtToken(string userId, string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddDays(int.Parse(jwtSettings["ExpiryInDays"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
