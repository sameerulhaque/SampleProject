using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Animal
{
    public class AnimalAttachmentModel
    {
        public required string AttachmentName { get; set; }
        public required int AnimalId { get; set; }
        public required IFormFile File { get; set; }
    }
}
