using ArabFootball.Shared.Helpers;
using ArabFootball.Api.Features.Auth.AuthDto;

namespace ArabFootball.Api.Features.Users
{
    public interface IAuthService
    {
        Task <ApiResponse<AuthResponseDto>> LoginAsync(string email, string password);
        Task <ApiResponse<AuthResponseDto>> LogoutAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);

        Task<ApiResponse<string>> RegisterAsync(RegisterDto dto);


    }

}

