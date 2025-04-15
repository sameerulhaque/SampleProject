using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Academy;
using SampleProject.Shared.Models.Inventory;

namespace SampleProject.API.Controllers
{
    public class InvoiceController : BaseController<Invoice, InvoiceModel>
    {
        public InvoiceController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<Invoice> service, IWriteRepository<Invoice> writeService, ICacheService cacheService) : base(configuration, webHostEnvironment, service, writeService, cacheService)
        {
        }
    }
}
