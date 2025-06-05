namespace SampleProject.Shared.Models.KYC
{
    public class RiskFieldModel
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string? Label { get; set; }
        public string? FieldType { get; set; } // Consider using enum
        public bool IsRequired { get; set; }
        public string? Placeholder { get; set; }
        public string? EndpointURL { get; set; }
        public int OrderIndex { get; set; }
        public List<RiskFieldValueMappingModel>? ValueMappings { get; set; }
    }

}
