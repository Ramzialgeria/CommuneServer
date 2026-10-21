using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using CommuneServer.Data.Entities;
using CommuneServer.Data;
using CommuneServer.Models;
using Microsoft.AspNet.Identity;

[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    private readonly CommuneDbContext _db;

    public AuthController()
    {
        _db = new CommuneDbContext();
    }

    [HttpPost]
    [Route("login")]
    public IHttpActionResult Login(
        LoginRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(
                "Username and password are required.");
        }

        var user = _db.U
            .FirstOrDefault(x =>
                x.Un == request.UserName &&
                x.IA);

        if (user == null)
            return Unauthorized();

        bool passwordValid;

        try
        {
            //passwordValid =
            //    BCrypt.Net.BCrypt.Verify(
            //        request.Password,
            //        user.PH);

            passwordValid = AspNetIdentityPasswordVerifier.Verify(
    user.PH,
    request.Password);
        }
        catch
        {
            passwordValid = false;
        }

        if (!passwordValid)
        {
            return Unauthorized();
        }

        var token =
            CreateToken(user);

        return Ok(new
        {
            accessToken = token,
            userName = user.Un,
            role = user.Rl,
            codeStructure = user.CS
        });
    }

    private string CreateToken(U user)
    {
        var secret =
            ConfigurationManager.AppSettings["JwtSecret"];

        var issuer =
            ConfigurationManager.AppSettings["JwtIssuer"];

        var audience =
            ConfigurationManager.AppSettings["JwtAudience"];

        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException(
                "JwtSecret is not configured.");

        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException(
                "JwtIssuer is not configured.");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException(
                "JwtAudience is not configured.");

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Un),

                new Claim(
                    ClaimTypes.Role,
                    user.Rl),

                new Claim(
                    "code_structure",
                    user.CS ?? "")
            };

        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _db.Dispose();
        }

        base.Dispose(disposing);
    }

}