using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public UserService(IConfiguration configuration, VuexyContext dbContext)
        {
            this.configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<APIResponseModel<string>> Login(LoginModel request, CancellationToken cancellationToken)
        {

            var isValidUser = false;
            isValidUser = await IsLoginSuccess(request, cancellationToken);

            if (!isValidUser)
            {
                throw new NotFoundException("Not found");
            }

            var token = string.Empty;
            await Task.Run(() =>
            {
                token = GenerateToken(request.Email);
            }, cancellationToken);
            var result = new APIResponseModel<string>(token);
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

            await _dbContext.Users
                .Where(x => x.StatusId == (int)UserStatusEnum.Deactivated)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.Role, y => string.Empty),cancellationToken);


            return true;
        }
        private async Task<bool> IsLoginSuccess(LoginModel request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => 
                    x.Email == request.Email && x.PasswordHash == request.Password,
                cancellationToken);
            if (user != null)
            {
                _dbContext.Entry(user).Property("LastAccessed").CurrentValue = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
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
