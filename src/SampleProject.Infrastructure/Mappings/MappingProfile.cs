
using AutoMapper;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Academy;
using SampleProject.Shared.Models.Inventory;
using SampleProject.Shared.Models.User;

namespace SampleProject.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //**CreateMap<TEntity, VEntity>();
            CreateMap<AcademyCourseModel, AcademyCourse>().ReverseMap();
            CreateMap<InvoiceModel, Invoice>().ReverseMap();
            CreateMap<UserModel, User>().ReverseMap();
            CreateMap<LoginModel, User>().ReverseMap();

        }

    }
}
