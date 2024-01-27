using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using User_microservices_example.Data.Models;

namespace User_microservices_example.Services;

public class AuthService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration)
    : IAuthService
{

    public async Task CreateRoles()
    {
        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    }
    public async Task<(int, string)> Registration(RegistrationModel model, string role)
    {
        var a_ver_que_sale = await userManager.FindByEmailAsync(model.Email!);

        var user = new IdentityUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        
        var successful = await userManager.CreateAsync(user, model.Password!);

        await userManager.AddToRoleAsync(user, role);

        return (1, "User created successfully!");
    }

    public async Task<(int, string)> Login(LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email!);
        if (user == null)
            return (0, "Invalid username");
        if (!await userManager.CheckPasswordAsync(user, model.Password!))
            return (0, "Invalid password");

        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("user_id", user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(
            userRoles.Select(userRole =>
                new Claim(ClaimTypes.Role, userRole))
        );

        return (1, GenerateToken(authClaims));
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["JwtSettings:Issuer"],
            Audience = configuration["JwtSettings:Audience"],
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}