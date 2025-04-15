using Microsoft.AspNetCore.Authorization;

namespace SampleProject.Infrastructure.Authentication
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "CustomAuthorize";
        private readonly string _subscriptionPackage = string.Empty;
        private readonly bool _isVerified = false;
        public CustomAuthorizeAttribute(string? subscriptionPackage, bool isVerified)
        {
            _subscriptionPackage = subscriptionPackage ?? "";
            _isVerified = isVerified;
            Policy = $"{POLICY_PREFIX}:{_subscriptionPackage}:{_isVerified}";
        }
        public CustomAuthorizeAttribute()
        {
            Policy = $"Default";
        }
    }
}