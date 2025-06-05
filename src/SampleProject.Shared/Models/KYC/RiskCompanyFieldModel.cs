namespace SampleProject.Shared.Models.KYC
{
    public class RiskCompanyFieldModel
    {
        public int Id { get; set; }
        public int CompanySectionId { get; set; }
        public int FieldId { get; set; }
        public int MaxScore { get; set; }
        public List<RiskCompanyFieldConditionModel>? Conditions { get; set; }
        public RiskFieldModel? Field { get; set; }
        public bool IsActive { get; set; }
    }

}
