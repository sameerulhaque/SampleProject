using Microsoft.AspNetCore.Http;
using SampleProject.Infrastructure.Services;
using SampleProject.Shared.Models.Global;

namespace SampleProject.Infrastructure.Authentication
{
    public static class UserInfoHelper
    {
        private static UserInfoService _userInfoService = null!;
        private static IHttpContextAccessor _httpContextAccessor = null!;

        public static void Initialize(UserInfoService userInfoService, IHttpContextAccessor httpContextAccessor)
        {
            _userInfoService = userInfoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static UserInfo GetUser()
        {
            if (_userInfoService == null)
            {
                throw new InvalidOperationException("UserInfoService has not been initialized.");
            }

            return _userInfoService.UserInfo ?? new UserInfo();
        }
        public static string IPAddress => _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
