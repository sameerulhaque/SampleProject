using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Animal
{
    public class AnimalRequestModel
    {
        public string Name { get; set; } = null!;

        public string Category { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public double Weight { get; set; }

        public string? Age { get; set; }

        public decimal Price { get; set; }

        public decimal PricePerShare { get; set; }

        public int TotalShares { get; set; }

        public int BookedShares { get; set; }

        public int? RemainingShares { get; set; }

        public string ImageUrl { get; set; } = null!;

        //public string? AdditionalImages { get; set; } = "";

        public string Description { get; set; } = null!;

        //public string? Features { get; set; } = string.Empty;
    }
}
