using Microsoft.AspNetCore.Authorization;

namespace SampleProject.Infrastructure.Authentication
{
    public class CustomAuthorizeRequirement : IAuthorizationRequirement
    {
        public string _subscriptionPackage { get; private set; } = string.Empty;
        public bool _isVerified { get; private set; } = false;

        public CustomAuthorizeRequirement() { }
        public CustomAuthorizeRequirement(string subscriptionPackage, bool isVerified) { _subscriptionPackage = subscriptionPackage; _isVerified = isVerified; }
    }
}
