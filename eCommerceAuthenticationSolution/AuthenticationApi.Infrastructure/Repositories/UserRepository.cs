using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository : IUser
    {
        private readonly AuthenticationDbContext authenticationDbContext;
        private readonly IConfiguration configuration;
        public UserRepository(AuthenticationDbContext _authenticationDbContext, IConfiguration _configuration)
        {
            authenticationDbContext = _authenticationDbContext;
            configuration = _configuration;
        }

        private async Task<AppUser> GetUserByEmail (string email)
        {
            var user = await authenticationDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user is null ? null! : user;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await authenticationDbContext.Users.FindAsync(userId);
            return user is not null ?
                new GetUserDTO
                (
                user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!
                ) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            try
            {
                var getUser = await GetUserByEmail(loginDTO.Email);
                if (getUser is null)
                    return new Response(false, "Invalid credentials");

                // Debugging: Print the stored hash
                Console.WriteLine($"Stored hash: {getUser.Password}");

                // Check if stored password is a valid BCrypt hash
                if (string.IsNullOrEmpty(getUser.Password) || !getUser.Password.StartsWith("$2"))
                {
                    return new Response(false, "Invalid stored password format");
                }

                // Verify password
                bool verifyPass = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
                if (!verifyPass)
                    return new Response(false, "Invalid credentials");

                string token = GenerateToken(getUser);
                return new Response(true, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return new Response(false, "An error occurred while processing the request.");
            }
        }


        private string GenerateToken(AppUser appUser)
        {
            var Key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
            var securitykey = new SymmetricSecurityKey(Key);
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,appUser.Name!),
                new(ClaimTypes.Email,appUser.Email!)
            };
            if (!string.IsNullOrEmpty(appUser.Role) || !Equals("string", appUser.Role))
                claims.Add(new(ClaimTypes.Role, appUser.Role!));

            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials
               );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var getUser = await GetUserByEmail(appUserDto.Email);
            if (getUser is not null)
            {
                return new Response(false, "You cannot use this email for registration");
            }

            // ✅ Hash the password before saving it
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(appUserDto.PassWord);

            var newUser = new AppUser()
            {
                Name = appUserDto.Name,
                Email = appUserDto.Email,
                Password = hashedPassword,  // ✅ Store the hashed password
                TelephoneNumber = appUserDto.TelephoneNumber,
                Address = appUserDto.Address,
                Role = appUserDto.Role
            };

            var result = authenticationDbContext.Users.Add(newUser);
            await authenticationDbContext.SaveChangesAsync();

            return result.Entity.Id > 0
                ? new Response(true, "User registered successfully")
                : new Response(false, "Invalid data provided");
        }

    }
}
