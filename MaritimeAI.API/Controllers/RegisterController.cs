using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DataAccessLayer.Entities;
using MaritimeAI.DtoLayer.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register(UserRegisterDto userRegisterDto)
        {

            if (string.IsNullOrEmpty(userRegisterDto.Username) ||
                string.IsNullOrEmpty(userRegisterDto.Password) ||
                string.IsNullOrEmpty(userRegisterDto.Email) ||
                string.IsNullOrEmpty(userRegisterDto.Lastname) ||
                string.IsNullOrEmpty(userRegisterDto.Name) ||
                string.IsNullOrEmpty(userRegisterDto.Phone))
            {
                return BadRequest("Lütfen Tüm Bilgileri Eksiksiz Doldurun!");
            }
            else { 

                _userService.TAdd(new User()
                {
                    Username = userRegisterDto.Username,
                    Password = userRegisterDto.Password,
                    Email = userRegisterDto.Email,
                    Lastname = userRegisterDto.Lastname,
                    Name = userRegisterDto.Name,
                    Phone = userRegisterDto.Phone
                });
                return Ok(new
                {
                    message = "Kayıt Başarılı. Giriş Yapılıyor!",
                    Username = userRegisterDto.Username,
                    Name = userRegisterDto.Name,
                    Lastname = userRegisterDto.Lastname    //JWT token ekleyeceğiz

                });

            }


        }
    }
}
