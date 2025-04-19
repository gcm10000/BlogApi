using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Constants;
using BlogApi.Application.Infrastructure.Identity.Configurations;
using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Application.Interfaces;
using BlogApi.Application.Migrations.IdentityDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogApi.Application.Infrastructure.Identity.Services;

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
            access_token = token,
            refresh_token = Guid.NewGuid().ToString(), // mock
            User = new UserDto
            {
                Id = user.AuthorId,
                Name = user.Name,
                IsMainTenancy = user.IsMainTenancy,
                TenancyDomainId = user.TenancyDomainId,
                TenancyDomainName = user.TenancyDomainName,
                Email = user.Email,
                Role = user.Role
            },
            ClientId = user.TenancyDomainId
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
        claims.Add(new Claim(CustomClaimTypes.TenancyDomainId, user.TenancyDomainId.ToString()));
        claims.Add(new Claim(CustomClaimTypes.IsMainTenancy, user.IsMainTenancy.ToString()));
        claims.Add(new Claim(CustomClaimTypes.AuthorId, user.AuthorId.ToString()));
        claims.Add(new Claim(CustomClaimTypes.PasswordChangeRequired, user.PasswordChangeRequired.ToString()));
        claims.Add(new Claim(CustomClaimTypes.TenancyDomainName, user.TenancyDomainName.ToString()));

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