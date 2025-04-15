using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Infrastructure.Services;
using SampleProject.Shared.Models.Global;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleProject.Infrastructure.Authentication
{
    public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizeRequirement>
    {
        private readonly UserInfoService _userInfoService;

        public CustomAuthorizationHandler(UserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizeRequirement requirement)
        {
            try
            {
                if (context.Resource is HttpContext httpContext)
                {
                    var headers = httpContext.Request.Headers;
                    var token = headers["Authorization"].ToString().Split()[1];
                    var handler = new JwtSecurityTokenHandler();
                    var principal = ValidateToken(token);
                    if (principal != null)
                    {
                        _userInfoService.UserInfo = MapClaimsToModel<UserInfo>(principal);
                    }
                    else
                    {
                        Console.WriteLine("Token is invalid or expired");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                context.Fail();
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("7LfchS5uM2UhNUHYJXZsaYcDkOEqzYbLp3G5EGKl4ks");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                //ClockSkew = TimeSpan.FromDays(200) // Optional: Adjust if you need a tolerance for token expiry
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
        public T MapClaimsToModel<T>(ClaimsPrincipal principal) where T : new()
        {
            var claims = principal?.Claims ?? Enumerable.Empty<Claim>();
            var model = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                var claim = claims.FirstOrDefault(c => c.Type.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (claim != null)
                {
                    var value = Convert.ChangeType(claim.Value, prop.PropertyType);
                    prop.SetValue(model, value);
                }
            }
            return model;
        }
    }

}
