using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Interfaces;
using SampleProject.Application.Services;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Dapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.KYC;

namespace SampleProject.API.Controllers
{
    [Route("api/risk/[controller]")]
    [ApiController]
    public class CompanySectionController : BaseController<RiskCompanySection, RiskCompanySectionModel, RiskCompanySection>
    {
        private readonly IKYCService _kycService;
        public CompanySectionController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<RiskCompanySection> service, IWriteRepository<RiskCompanySection> writeService, ICacheService cacheService, IKYCService kycService) : base(configuration, webHostEnvironment, service, writeService, cacheService, cacheTime: 0)
        {
            _kycService = kycService;
        }


        [HttpGet("risk-configurations")]
        public async Task<IActionResult> GetRiskConfiguration(int? companyId, CancellationToken cancellationToken)
        {
            var res = await _kycService.GetRiskConfiguration(companyId, cancellationToken);
            return ApiResult(res);
        }

        [HttpPost("risk-configurations")]
        public async Task<IActionResult> SaveRiskConfiguration(int? companyId, [FromBody] RiskConfigurationModel riskConfiguration, CancellationToken cancellationToken)
        {
            var res = await _kycService.SaveRiskConfiguration(companyId, riskConfiguration, cancellationToken);
            return ApiResult(res);
        }
    }
}
