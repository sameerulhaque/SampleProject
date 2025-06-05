namespace SampleProject.Shared.Models.KYC
{
    public class RiskUserAssessmentSectionScoreModel
    {
        public int SectionId { get; set; }
        public int CompanySectionId { get; set; }
        public string SectionName { get; set; }
        public int Score { get; set; }
        public int MaxPossible { get; set; }
    }

}
