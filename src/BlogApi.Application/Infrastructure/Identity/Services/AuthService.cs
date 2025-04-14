using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Constants;
using BlogApi.Application.Infrastructure.Identity.Configurations;
using BlogApi.Application.Interfaces;
using BlogApi.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogApi.Infrastructure.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly JwtOptions _jwtOptions;


    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded) return null;

        var dateExpiration = DateTime.Now.AddMinutes(_jwtOptions.Expiration);

        var token = await GenerateAccessTokenAsync(user, dateExpiration);

        return new AuthResponseDto
        {
            Access_Token = token,
            Refresh_Token = Guid.NewGuid().ToString(), // mock
            User = new UserDto
            {
                Id = user.AuthorId,
                Username = user.UserName,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Implementar lógica para validar e renovar token
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        return result.Succeeded;
    }

    //private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
    //{
    //    var claims = await GetApplicationUserClaimsAsync(user);

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtOptions:SecurityKey"]));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(_config["JwtOptions:Issuer"],
    //                                     _config["JwtOptions:Issuer"],
    //                                     claims,
    //                                     expires: DateTime.Now.AddHours(1),
    //                                     signingCredentials: creds);

    //    return new AuthResponseDto
    //    {
    //        Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
    //        Refresh_Token = Guid.NewGuid().ToString(), // mock
    //        User = new UserDto
    //        {
    //            Id = user.AuthorId,
    //            Username = user.UserName,
    //            Name = user.Name,
    //            Email = user.Email,
    //            Role = user.Role
    //        }
    //    };
    //}

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user, DateTime dateExpiration)
    {
        var tokenClaims = await GetApplicationUserClaimsAsync(user);

        return TokenGenerator(dateExpiration, tokenClaims);
    }

    private string TokenGenerator(DateTime dateExpiration, IList<Claim> tokenClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = dateExpiration,
            SigningCredentials = _jwtOptions.SigningCredentials,
            Audience = _jwtOptions.Audience,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }


    private async Task<IList<Claim>> GetApplicationUserClaimsAsync(ApplicationUser user)
    {
        var claims = (await _userManager.GetClaimsAsync(user)).ToList();
        var roles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

        if (user.Name is not null)
            claims.Add(new Claim(CustomClaimTypes.Name, user.Name));

        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        return claims;
    }
}