using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;

namespace CommuneServer.Security
{
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var authHeader =
                    request.Headers.Authorization;

                // No Authorization header.
                // Let [Authorize] decide whether authentication
                // is required for this endpoint.
                if (authHeader == null ||
                    !string.Equals(
                        authHeader.Scheme,
                        "Bearer",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return await base.SendAsync(
                        request,
                        cancellationToken);
                }

                var token = authHeader.Parameter;

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized();
                }

                ClaimsPrincipal principal;

                if (!TryValidateToken(token, out principal))
                {
                    return Unauthorized();
                }

                // Make the authenticated user available to Web API.
                request.GetRequestContext().Principal = principal;

                Thread.CurrentPrincipal = principal;

                return await base.SendAsync(
                    request,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "JWT ERROR: " + ex);

                return Unauthorized();
            }
        }

        private bool TryValidateToken(
            string token,
            out ClaimsPrincipal principal)
        {
            principal = null;

            var secret =
                ConfigurationManager.AppSettings["JwtSecret"];

            var issuer =
                ConfigurationManager.AppSettings["JwtIssuer"];

            var audience =
                ConfigurationManager.AppSettings["JwtAudience"];

            if (string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return false;
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret));

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = key,

                    ValidateIssuer = true,

                    ValidIssuer = issuer,

                    ValidateAudience = true,

                    ValidAudience = audience,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.FromMinutes(1),

                    NameClaimType = ClaimTypes.Name,

                    RoleClaimType = ClaimTypes.Role
                };

            try
            {
                var handler =
                    new JwtSecurityTokenHandler();

                SecurityToken validatedToken;

                principal =
                    handler.ValidateToken(
                        token,
                        validationParameters,
                        out validatedToken);

                return principal != null &&
                       principal.Identity != null &&
                       principal.Identity.IsAuthenticated;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private HttpResponseMessage Unauthorized()
        {
            return new HttpResponseMessage(
                HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "Invalid or expired token"
            };
        }
    }
}