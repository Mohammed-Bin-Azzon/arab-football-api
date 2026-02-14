using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using System.Net;
using ArabFootball.Api.Shared.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using ArabFootball.Api.Features.Auth.AuthDto;



namespace ArabFootball.Api.Features.Users
{
    public class AuthService : IAuthService
    {
        public AppDBContext _context;
        public AuthService(AppDBContext context) 
        { 
            _context = context;
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Username == username);

            if (user == null)
                return ApiResponse<AuthResponseDto>.Error(HttpStatusCode.Unauthorized, "Invalid email or password");

            var isPasswordValid = VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid)
                return ApiResponse<AuthResponseDto>.Error(HttpStatusCode.Unauthorized, "Invalid email or password");
                

            var userDto = new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email,
            };
              
            return ApiResponse<AuthResponseDto>.Success(userDto, "You are login successfully");
        }

        public async Task<ApiResponse<AuthResponseDto>> LogoutAsync(int id)
        {
           

            var userDto = new AuthResponseDto();
            return ApiResponse<AuthResponseDto>.Success(userDto, "OK");
        }


        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }



        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                return ApiResponse<string>.Error(HttpStatusCode.BadRequest, "Username is already taken");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return ApiResponse<string>.Error(HttpStatusCode.BadRequest, "Email is already taken");


            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password)
            };

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();   

            return ApiResponse<string>.Success("Registered successfully");
        }
    }
}
