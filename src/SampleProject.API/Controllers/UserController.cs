using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;

namespace SampleProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController<User, UserModel>
    {
        private readonly IUserService _userService;
        public UserController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<User> service, IWriteRepository<User> writeService, ICacheService cacheService, IUserService userService) : base(configuration, webHostEnvironment, service, writeService, cacheService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new BadRequestException("Request Error", new Dictionary<string, string[]>());

            var res = await _userService.Login(entity, cancellationToken);
            return ApiResult(res);
        }

    }
}
