using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Shared.Models.Academy;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.Inventory;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;
using System.Data.Entity.Infrastructure;

namespace SampleProject.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserBookingController : BaseController<UserBooking, UserBookingRequestModel, UserBookingResponseModel>
    {
        private readonly IUserService _userService;
        private readonly IDapperDbConnectionFactory _dbConnectionFactory;
        private readonly DapperQueryBuilder _queryBuilder;

        public UserBookingController(IDapperDbConnectionFactory dbConnectionFactory, DapperQueryBuilder queryBuilder, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<UserBooking> service, IWriteRepository<UserBooking> writeService, ICacheService cacheService, IUserService userService) : base(configuration, webHostEnvironment, service, writeService, cacheService, cacheTime: 0)
        {
            _userService = userService;
            _dbConnectionFactory = dbConnectionFactory;
            _queryBuilder = queryBuilder;
        }


        [HttpPost("add-bookings")]
        public async Task<IActionResult> AddBookings([FromBody] UserBookingsRequestModel entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new BadRequestException("Request Error", new Dictionary<string, string[]>());

            var res = await _userService.AddUserBooking(entity, cancellationToken);
            return ApiResult(res);
        }

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, string status, CancellationToken cancellationToken)
        {
            var res = await _userService.ChangeStatus(id, status, cancellationToken);
            return ApiResult(res);
        }
    }
}
