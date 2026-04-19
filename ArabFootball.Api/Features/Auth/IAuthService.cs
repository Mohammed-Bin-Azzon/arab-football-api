using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse<object>> LogoutAsync();

        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}