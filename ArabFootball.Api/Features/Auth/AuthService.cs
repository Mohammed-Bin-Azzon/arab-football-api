using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
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

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var username = dto.Username.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new InvalidOperationException("اسم المستخدم مستخدم بالفعل.");

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل.");

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

            var token = GenerateJwtToken(fan);

            return new AuthResponseDto
            {
                UserId = fan.Id,
                Username = fan.Username,
                Email = fan.Email,
                Role = fan.Role.ToString(),
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("بيانات الدخول غير صحيحة.");

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

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT audience is missing.");

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