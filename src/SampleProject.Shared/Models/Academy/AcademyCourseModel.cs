using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Academy
{
    public class AcademyCourseModel
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool? IsEnrolled { get; set; }
    }
}
