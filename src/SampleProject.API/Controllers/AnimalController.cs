using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Academy;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.Inventory;
using SampleProject.Shared.Models.Misc;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;



namespace SampleProject.API.Controllers
{
    public class AnimalController : BaseController<Animal, AnimalRequestModel, Animal>
    {
        public AnimalController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<Animal> service, IWriteRepository<Animal> writeService, ICacheService cacheService) : base(configuration, webHostEnvironment, service, writeService, cacheService)
        {
        }


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] AnimalAttachmentModel model)
        {
            var path = GetCustomerDocsFileAndPathName(model.File.FileName);
            try
            {
                using var image = await Image.LoadAsync(model.File.OpenReadStream());

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(800, 0)
                }));
                var encoder = new JpegEncoder { Quality = 75 };
                await image.SaveAsync(path.Item2, new PngEncoder { CompressionLevel = PngCompressionLevel.Level6 });
                var result = new APIResponseModel<AnimalAttachmentModel>(model);
                result.OK();
                return ApiResult(result);
            }
            catch
            {
                System.IO.File.Delete(path.Item2);
                throw;
            }
        }
    }
}
