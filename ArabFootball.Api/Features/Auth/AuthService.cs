using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArabFootball.Api.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var username = dto.Username.Trim();
                var email = dto.Email.Trim().ToLowerInvariant();

                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    return ApiResponse<AuthResponseDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "اسم المستخدم مستخدم بالفعل.");
                }

                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    return ApiResponse<AuthResponseDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "البريد الإلكتروني مستخدم بالفعل.");
                }

                var fan = new Fan
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(dto.Password),
                    Role = UserRole.Fan,
                    DisplayName = username,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Fans.AddAsync(fan);
                await _context.SaveChangesAsync();

                var authResponse = BuildAuthResponse(fan);

                return ApiResponse<AuthResponseDto>.Success(
                    authResponse,
                    "تم إنشاء الحساب بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<AuthResponseDto>.Fail(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إنشاء الحساب.");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                var email = dto.Email.Trim().ToLowerInvariant();

                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                {
                    return ApiResponse<AuthResponseDto>.Fail(
                        HttpStatusCode.Unauthorized,
                        "بيانات الدخول غير صحيحة.");
                }

                var authResponse = BuildAuthResponse(user);

                return ApiResponse<AuthResponseDto>.Success(
                    authResponse,
                    "تم تسجيل الدخول بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<AuthResponseDto>.Fail(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تسجيل الدخول.");
            }
        }

        public Task<ApiResponse<object>> LogoutAsync()
        {
            // إذا لم يكن عندك refresh token / blacklist بعد، فهذا يكفي حاليًا
            return Task.FromResult(
                ApiResponse<object>.Success(
                    null,
                    "تم تسجيل الخروج بنجاح."));
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("JWT configuration is missing.");
            }

            var expiresInMinutesString = _configuration["Jwt:ExpiresInMinutes"];
            if (!int.TryParse(expiresInMinutesString, out var expiresInMinutes))
                expiresInMinutes = 60;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}