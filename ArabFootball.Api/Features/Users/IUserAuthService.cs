using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using ArabFootball.Api.Features.Users.UsersDto;

namespace ArabFootball.Api.Features.Users
{
    public interface IUserAuthService
    {
        Task <ApiResponse<UserResponseDto>> LoginAsync(string email, string password);
        Task <ApiResponse<UserResponseDto>> LogoutAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);

        //Task<ApiResponse<string>> RegisterAsync(RegisterFanDto dto);


    }

}

