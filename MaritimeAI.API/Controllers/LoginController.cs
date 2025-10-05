using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DtoLayer.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult UserList()
        {
            var values = _userService.TGetAll();
            return Ok(values);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDto userLoginDto)
        {

            if (string.IsNullOrEmpty(userLoginDto.Username) ||
                string.IsNullOrEmpty(userLoginDto.Password))
            {
                return BadRequest("Kullanıcı adı ve şifre gerekli");
            }

            var user = _userService.TGetAll().FirstOrDefault(x => x.Password == userLoginDto.Password && x.Username == userLoginDto.Username);

            if (user == null)
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı");
            }

            return Ok(new
            {
                message = "Giriş Başarılı",
                Username = user.Username,
                Name = user.Name,
                Lastname = user.Lastname    //JWT token ekleyeceğiz

            });

            return BadRequest();
        }
    }
}
