
using AutoMapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models;
using SampleProject.Shared.Models.Academy;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.Inventory;
using SampleProject.Shared.Models.KYC;
using SampleProject.Shared.Models.User;

namespace SampleProject.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AnimalRequestModel, Animal>().ReverseMap();
            CreateMap<UserBookingRequestModel, UserBooking>().ReverseMap();
            CreateMap<UserBooking, UserBookingResponseModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
            CreateMap<UserRequestModel, User>().ReverseMap();
            CreateMap<LoginModel, User>().ReverseMap();
            CreateMap<LoginResponseModel, User>().ReverseMap();
            CreateMap<RegisterRequestModel, User>().ReverseMap();
            CreateMap <CompanyModel, Company>().ReverseMap();
            CreateMap<RiskCompanyFieldConditionModel, RiskCompanyFieldCondition>().ReverseMap();
            CreateMap<RiskCompanyFieldModel, RiskCompanyField>()
            .ForMember(dest => dest.RiskCompanyFieldConditions, opt => opt.MapFrom(src => src.Conditions))
            .ReverseMap();
            CreateMap<RiskCompanySectionModel, RiskCompanySection>()
            .ForMember(dest => dest.RiskCompanyFields, opt => opt.MapFrom(src => src.Fields))
            .ReverseMap();
            CreateMap <RiskFieldModel, RiskField>().ReverseMap();
            CreateMap <RiskFieldValueMappingModel, RiskFieldValueMapping>().ReverseMap();
            CreateMap <RiskSectionModel, RiskSection>().ReverseMap();
            CreateMap <RiskConfigurationModel, RiskConfiguration>().ReverseMap();
        }

    }
}
