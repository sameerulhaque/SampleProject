namespace SampleProject.Shared.Models.KYC
{
    public class RiskScoreSectionScoreModel
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int Score { get; set; }
        public int MaxPossible { get; set; }
    }

}
