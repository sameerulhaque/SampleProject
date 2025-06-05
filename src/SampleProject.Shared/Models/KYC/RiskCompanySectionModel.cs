namespace SampleProject.Shared.Models.KYC
{
    public class RiskCompanySectionModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SectionId { get; set; }
        public bool IsActive { get; set; }
        public int Weightage { get; set; }
        public List<RiskCompanyFieldModel>? Fields { get; set; }
        public RiskSectionModel? Section { get; set; }
    }

}
