using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Academy;

namespace SampleProject.API.Controllers
{
    public class AcademyCourseController : BaseController<AcademyCourse, AcademyCourseModel>
    {
        public AcademyCourseController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<AcademyCourse> service, IWriteRepository<AcademyCourse> writeService, ICacheService cacheService) : base(configuration, webHostEnvironment, service, writeService, cacheService)
        {
        }
    }
}
