// Controllers/RegisterController.cs
using Microsoft.AspNetCore.Mvc;

namespace gps_jamming_classifier_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register([FromBody] UserRegistration registration)
        {
            // Kiểm tra thông tin và tạo tài khoản trong database
            // Trả về kết quả
            return Ok(new { Success = true });
        }
    }

    public class UserRegistration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
