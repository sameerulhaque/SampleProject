namespace SampleProject.Shared.Models.KYC
{
    public class RiskUserAssessmentSectionModel
    {
        public int SectionId { get; set; }
        public RiskSectionModel Section { get; set; }
        public List<RiskUserAssessmentFieldValueModel> Fields { get; set; }
        public int Score { get; set; }
        public int MaxPossible { get; set; }
    }

}
