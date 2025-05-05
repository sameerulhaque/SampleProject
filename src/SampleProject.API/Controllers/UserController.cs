using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Shared.Models;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;

namespace SampleProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController<User, UserModel>
    {
        private readonly IUserService _userService;
        private readonly IDapperDbConnectionFactory _dbConnectionFactory;
        private readonly DapperQueryBuilder _queryBuilder;
        public UserController(IDapperDbConnectionFactory dbConnectionFactory, DapperQueryBuilder queryBuilder, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<User> service, IWriteRepository<User> writeService, ICacheService cacheService, IUserService userService) : base(configuration, webHostEnvironment, service, writeService, cacheService)
        {
            _userService = userService;
            _dbConnectionFactory = dbConnectionFactory;
            _queryBuilder = queryBuilder;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new BadRequestException("Request Error", new Dictionary<string, string[]>());

            var res = await _userService.Login(entity, cancellationToken);
            return ApiResult(res);
        }


        [HttpGet("dapper-test")]
        public async Task<IActionResult> DapperTest(CancellationToken cancellationToken)
        {
            var request = new DataListerRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Sort = "Users.CreatedAt DESC",
                PageId = 209,
                idColumn = "Id",
                Filters = new List<PageFilterModel>
                    {
                        new PageFilterModel
                        {
                            Field = "StatusId",
                            Operator = "equals",
                            Value = "1",
                            Operation = "AND",
                            ExternalSearch = true,
                            TryConvertType = "bool",
                            FixedFilter = true
                        }
                    }
                };


            return Ok(_queryBuilder.GetPageLister(request));
        }


    }
}
