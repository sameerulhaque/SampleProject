using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Infrastructure.Mappings;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace SampleProject.Application.Services
{
    public enum UserStatusEnum
    {
        Activated = 1,Deactivated = 2
    }
    public class UserWithLastLogin : User
    {
        public DateTime? LastAccessed { get; set; }
    }
    public class UserService : IUserService
    {
        protected readonly IConfiguration configuration;
        private readonly VuexyContext _dbContext;
        private readonly IMapper _mapper;
        protected readonly ICacheService _cacheService;



        public UserService(IConfiguration configuration, VuexyContext dbContext, IMapper mapper, ICacheService cacheService)
        {
            this.configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        public async Task<APIResponseModel<LoginResponseModel>> Login(LoginModel request, CancellationToken cancellationToken)
        {
            var user = await IsLoginSuccess(request, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("Not found");
            }

            var token = string.Empty;
            await Task.Run(() =>
            {
                token = GenerateToken(request.Email);
            }, cancellationToken);
            var entity = _mapper.Map<LoginResponseModel>(user);
            entity.Token = token;

            var result = new APIResponseModel<LoginResponseModel>(entity);
            result.OK();
            return result;
        }

        public async Task<APIResponseModel<LoginResponseModel>> RegisterUser(RegisterRequestModel request, CancellationToken cancellationToken)
        {
            var phone = request.Phone?.Trim()?.ToLower();
            var email = request.Email?.Trim()?.ToLower();
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email || x.Phone == phone, cancellationToken);
            if (user == null)
            {
                user = _mapper.Map<User>(request);
                user.PasswordHash = request.Password;
                if (user is TrackableEntity trackableEntity)
                {
                    trackableEntity.Created("System");
                }
                await _dbContext.Set<User>().AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                user.Phone = request.Phone;
                user.Address = request.Address;
                await _dbContext.SaveChangesAsync();
            }

            var token = string.Empty;
            await Task.Run(() =>
            {
                token = GenerateToken(user.Email);
            }, cancellationToken);
            var entity = _mapper.Map<LoginResponseModel>(user);
            entity.Token = token;

            var result = new APIResponseModel<LoginResponseModel>(entity);
            result.OK();
            return result;
        }

        public async Task<APIResponseModel<bool>> ChangeStatus(int id, string status, CancellationToken cancellationToken)
        {
            var booking = _dbContext.UserBookings.FirstOrDefault(x => x.Id == id);
            if(booking != null)
            {
                booking.Status = status;
                _dbContext.SaveChanges();
                var result = new APIResponseModel<bool>(true);
                result.OK();
                return result;
            }
            var resultError = new APIResponseModel<bool>(false);
            resultError.OK();
            return resultError;
        }


        public async Task<APIResponseModel<LoginResponseModel>> AddUserBooking(UserBookingsRequestModel request, CancellationToken cancellationToken)
        {
            var res = await RegisterUser(request.User, cancellationToken);
            if (res.Value?.Id > 0)
            {
                var bookings = _mapper.Map<List<UserBooking>>(request.Bookings);

                var animalIds = request.Bookings.Select(x => x.AnimalId);
                var animals = _dbContext.Animals.Where(x => animalIds.Contains(x.Id));
                List<UserBooking> dbbookings = new List<UserBooking>();
                foreach (var booking in bookings)
                {
                    var animal = animals.FirstOrDefault(x => x.Id == booking.AnimalId);
                    if (animal != null)
                    {
                        animal.BookedShares += booking.Shares;
                        animal.RemainingShares -= booking.Shares;
                    }
                    booking.BookingDate = DateTime.Now;
                    booking.Status = "pending";
                    booking.UserId = res.Value.Id;
                    if (booking is TrackableEntity trackableEntity)
                    {
                        trackableEntity.Created("System");
                    }
                    dbbookings.Add(booking);
                }
                await _dbContext.UserBookings.AddRangeAsync(dbbookings);
                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveByCondition(key => key.Contains("EF.Entities.Animal"));

                return res;
            }
            throw new NotFoundException("Not found");
        }
        public async Task<APIResponseModel<List<LoginResponseModel>>> GetBookingUsers(int animalId, int userId, CancellationToken cancellationToken)
        {
            var bookings = await _dbContext.UserBookings
                .Where(x => (animalId > 0 ? (animalId == x.AnimalId) : true))
                .Where(x => (userId > 0 ? (userId == x.UserId) : true))
                .Select(x => new LoginResponseModel()
                {
                    Id = x.UserId,
                    FullName = x.User.FullName,
                    Shares = x.Shares,
                    BookingDate = x.BookingDate,
                    Status = x.Status
                }).ToListAsync();


            var result = new APIResponseModel<List<LoginResponseModel>>(bookings);
            result.OK();
            return result;

        }




        private string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "AlternativeKey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<string> permissions = [];
            if (username.Equals("MyUsername"))
            {
                permissions.Add("CanDelete");
            }
            var permissionClaims = permissions.Select(value => new Claim("Permissions", value));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,username)
        };

            claims = [.. claims, .. permissionClaims];

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Key"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> ValidateUsers(LoginModel request, CancellationToken cancellationToken)
        {
            await _dbContext.Users
                .IgnoreQueryFilters()
                .Where(x => x.IsDeleted == true)
                .ExecuteDeleteAsync(cancellationToken);

            //await _dbContext.Users
            //    .Where(x => x.StatusId == (int)UserStatusEnum.Deactivated)
            //    .ExecuteUpdateAsync(x => x.SetProperty(y => y.Role, y => string.Empty),cancellationToken);


            return true;
        }
        private async Task<User> IsLoginSuccess(LoginModel request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == request.Email && x.PasswordHash == request.Password, cancellationToken);
            if (user != null)
            {
                _dbContext.Entry(user).Property("LastAccessed").CurrentValue = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return user;
            }
            return null;
        }


        public static readonly Func<VuexyContext, int?, Task<List<UserWithLastLogin>>> _compiledUserAndPermissions =
            EF.CompileAsyncQuery((VuexyContext context, int? userId) =>
                context.Users
                .TagWith("Fetching active users with permissions")
                .Include(x => x.UserPermissions)
                .Where(x => userId == null || x.Id == userId)
                .AsNoTracking()
                .AsSplitQuery()
                .Select(user => new UserWithLastLogin
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    UserPermissions = user.UserPermissions,
                    LastAccessed = EF.Property<DateTime?>(user, "LastAccessed")
                })
                .ToList()
        );


    }
}
