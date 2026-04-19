using ArabFootball.Api.Features.Auth;
using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return this.ValidationProblemResponse("بيانات التسجيل غير صالحة.");

            var response = await _authService.RegisterAsync(dto);
            return this.ToActionResult(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return this.ValidationProblemResponse("بيانات تسجيل الدخول غير صالحة.");

            var response = await _authService.LoginAsync(dto);
            return this.ToActionResult(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _authService.LogoutAsync();
            return this.ToActionResult(response);
        }
    }
}