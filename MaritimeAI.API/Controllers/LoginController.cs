using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DtoLayer.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }



        [HttpPost]
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


            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var token = new JwtSecurityToken(

                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Giriş Başarılı",
                token = tokenString,
                Username = user.Username,
                Name = user.Name,
                Lastname = user.Lastname

            });

        }




    }
}
