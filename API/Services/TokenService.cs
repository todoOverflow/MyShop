using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService
{
    private readonly UserManager<User> _userMnaager;
    private readonly IConfiguration _config;
    public TokenService(UserManager<User> userMnaager, IConfiguration config)
    {
        _config = config;
        _userMnaager = userMnaager;
    }

    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName)
        };

        var roles = await _userMnaager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
          issuer: null,
          audience: null,
          claims: claims,
          signingCredentials: credentials,
          expires: DateTime.Now.AddDays(3)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
