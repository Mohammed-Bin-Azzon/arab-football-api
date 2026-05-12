using ArabFootball.Api.Features.Auth;
using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
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
            //if (!ModelState.IsValid)
            //    return this.ValidationProblemResponse("بيانات التسجيل غير صالحة.");

            return Response(await _authService.RegisterAsync(dto));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            //if (!ModelState.IsValid)
            //    return this.ValidationProblemResponse("بيانات تسجيل الدخول غير صالحة.");

            return Response(await _authService.LoginAsync(dto));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Response(await _authService.LogoutAsync());
        }
    }
}