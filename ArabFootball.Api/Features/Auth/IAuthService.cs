using ArabFootball.Api.Features.Auth.AuthDto;

namespace ArabFootball.Api.Features.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}