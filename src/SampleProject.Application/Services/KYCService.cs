using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SampleProject.Application.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Shared.Models.KYC;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;
using static System.Collections.Specialized.BitVector32;

namespace SampleProject.Application.Services
{
    public class KYCService : IKYCService
    {
        //private readonly RestClient _client;
        protected readonly IConfiguration _configuration;
        private readonly VuexyContext _context;
        private readonly IMapper _mapper;
        protected readonly ICacheService _cacheService;


        public KYCService( IConfiguration configuration, VuexyContext dbContext, IMapper mapper, ICacheService cacheService)
        {
            _configuration = configuration;
            _context = dbContext;
            _mapper = mapper;
            _cacheService = cacheService;
        }


        public async Task<APIResponseModel<RiskConfigurationModel>> GetRiskConfiguration(int? companyId, CancellationToken cancellationToken)
        {
            int companyIdNumber = companyId ?? 1;
            var sections = await _context.RiskSections
                .Include(x => x.RiskFields).ThenInclude(x => x.RiskFieldValueMappings)
                .AsNoTracking()
                .ToListAsync();

            int sectionWeight = sections.Count > 0 ? (int)Math.Floor(100.0 / sections.Count) : 0;

            var companySections = new List<RiskCompanySectionModel>();

            foreach (var section in sections)
            {
                var companySection = new RiskCompanySectionModel
                {
                    CompanyId = companyId ?? 0,
                    SectionId = section.Id,
                    IsActive = true,
                    Weightage = sectionWeight,
                    Section = _mapper.Map<RiskSectionModel>(section),
                    Fields = new List<RiskCompanyFieldModel>()
                };
                if (section.RiskFields.Any())
                {
                    foreach (var field in section.RiskFields)
                    {
                        var companyField = new RiskCompanyFieldModel
                        {
                            FieldId = field.Id,
                            IsActive = true,
                            MaxScore = 100,
                            Field = _mapper.Map<RiskFieldModel>(field),
                            Conditions = field.RiskFieldValueMappings?.Select(mapping => new RiskCompanyFieldConditionModel
                            {
                                FieldValueMappingId = mapping.Id,
                                Operator = "=",
                                RiskScore = 0,
                                FieldValueMapping = _mapper.Map<RiskFieldValueMappingModel>(mapping),
                                IsActive = true
                            }).ToList() ?? new List<RiskCompanyFieldConditionModel>()
                        };

                        companySection.Fields.Add(companyField);
                    }
                }

                companySections.Add(companySection);
            }

            var config = new RiskConfigurationModel
            {
                Name = $"",
                Version = "1.0.0",
                CompanyId = companyId ?? 0,
                CompanySections = companySections
            };

            var result = new APIResponseModel<RiskConfigurationModel>(config);
            result.OK();
            return result;
        }


        public async Task<APIResponseModel<RiskConfigurationModel>> SaveRiskConfiguration(int? companyId, RiskConfigurationModel request, CancellationToken cancellationToken)
        {
            var configurationModel = _mapper.Map<RiskConfiguration>(request);
            var model = _mapper.Map<List<RiskCompanySection>>(request.CompanySections);


            var sectionIds = request.CompanySections?.Select(x => x.SectionId);
            var companySections = await _context.RiskCompanySections.Where(x => x.CompanyId == companyId && (sectionIds != null ? sectionIds.Contains(x.SectionId) : true))
                .ToListAsync(cancellationToken)
                ?? new List<RiskCompanySection>();
            foreach (var companySection in companySections)
            {

            }


            var result = new APIResponseModel<RiskConfigurationModel>(request);
            result.OK();
            return result;
        }

    }
}
