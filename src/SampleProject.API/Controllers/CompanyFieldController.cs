using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Interfaces;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.KYC;

namespace SampleProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyFieldController : BaseController<RiskCompanyField, RiskCompanyFieldModel, RiskCompanyField>
    {
        public CompanyFieldController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<RiskCompanyField> service, IWriteRepository<RiskCompanyField> writeService, ICacheService cacheService) : base(configuration, webHostEnvironment, service, writeService, cacheService, cacheTime: 0)
        {

        }
    }
}
