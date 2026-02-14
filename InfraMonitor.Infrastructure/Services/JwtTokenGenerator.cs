using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Common.Models;
using InfraMonitor.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfraMonitor.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    => _jwtSettings = jwtSettings.Value;


    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = CreateClaims(user);

        var token = CreateJwtSecurityToken(claims, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    #region  Private Methods

    private JwtSecurityToken CreateJwtSecurityToken(List<Claim> claims, SigningCredentials credentials)
    => new (
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);
    

    private List<Claim> CreateClaims(User user)
     => new()
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.Name)
    };
    #endregion
}
