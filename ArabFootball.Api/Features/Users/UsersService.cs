using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using System.Net;
using ArabFootball.Api.Features.Users.UsersDto;
using ArabFootball.Api.Shared.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;



namespace ArabFootball.Api.Features.Users
{
    public class UsersService : IUserAuthService
    {
        public AppDBContext _context;
        public UsersService(AppDBContext context) 
        { 
            _context = context;
        }

        public async Task<ApiResponse<UserResponseDto>> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Username == username);

            if (user == null)
                return ApiResponse<UserResponseDto>.Error(HttpStatusCode.Forbidden, "Invalid email or password");

            var isPasswordValid = VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid)
                return ApiResponse<UserResponseDto>.Error(HttpStatusCode.Forbidden, "Invalid email or password");
                

            var userDto = new UserResponseDto
            {
                Username = user.Username,
                Email = user.Email,
            };
              
            return ApiResponse<UserResponseDto>.Success(userDto, "You are login successfully");
        }

        public async Task<ApiResponse<UserResponseDto>> LogoutAsync(int id)
        {
           

            var userDto = new UserResponseDto();
            return ApiResponse<UserResponseDto>.Success(userDto, "OK");
        }


        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }



        //public async Task RegisterAsync(RegisterFanDto dto)
        //{
        //    var emailExists = await _context.Set<User>()
        //        .AnyAsync(u => u.Email == dto.Email);

        //    if (emailExists)
        //        return ApiResponse<RegisterFanDto>.Error(HttpStatusCode.Forbidden,"Email already exists");

        //    var fan = new Fan
        //    {
        //        Username = dto.Username,
        //        Email = dto.Email,
        //        PasswordHash = _authService.HashPassword(dto.Password),
        //        Points = 0
        //    };

        //    _context.Set<Fan>().Add(fan);
        //    await _context.SaveChangesAsync();
        //}
    }
}
