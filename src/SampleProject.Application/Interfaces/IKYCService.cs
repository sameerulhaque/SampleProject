using SampleProject.Shared.Models.KYC;
using SampleProject.Shared.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Application.Interfaces
{
    public interface IKYCService
    {
        Task<APIResponseModel<RiskConfigurationModel>> GetRiskConfiguration(int? companyId, CancellationToken cancellationToken);
        Task<APIResponseModel<RiskConfigurationModel>> SaveRiskConfiguration(int? companyId, RiskConfigurationModel request, CancellationToken cancellationToken);


    }
}
