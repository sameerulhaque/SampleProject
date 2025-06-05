namespace SampleProject.Shared.Models.KYC
{
    public class RiskConfigurationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int CompanyId { get; set; }
        public List<RiskCompanySectionModel>? CompanySections { get; set; }
    }

}
