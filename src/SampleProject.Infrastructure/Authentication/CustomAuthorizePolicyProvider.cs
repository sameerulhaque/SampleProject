using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SampleProject.Infrastructure.Authentication
{
    public class CustomAuthorizePolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        public CustomAuthorizePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
                                FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
                                FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            try
            {
                if (policyName.StartsWith("CustomAuthorize", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = policyName.Substring("CustomAuthorize:".Length).Split(':');
                    if (parts.Length == 2 &&
                        bool.TryParse(parts[1], out var isVerified))
                    {
                        var subscriptionPackage = parts[0];

                        var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                        policy.AddRequirements(new CustomAuthorizeRequirement(subscriptionPackage, isVerified));
                        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
                    }
                    throw new InvalidOperationException("");
                }
                else
                {
                    var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                    policy.AddRequirements(new CustomAuthorizeRequirement());
                    return Task.FromResult<AuthorizationPolicy?>(policy.Build());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
