namespace SampleProject.Shared.Models.KYC
{
    public class RiskCompanyFieldConditionModel
    {
        public int Id { get; set; }
        public int CompanyFieldId { get; set; }
        public int? FieldValueMappingId { get; set; }
        public string? Operator { get; set; }
        public string? Value { get; set; }
        public string? ValueTo { get; set; }
        public int RiskScore { get; set; }
        public RiskFieldValueMappingModel? FieldValueMapping { get; set; }
        public bool IsActive { get; set; }
    }

}
