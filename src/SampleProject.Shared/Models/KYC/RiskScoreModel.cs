namespace SampleProject.Shared.Models.KYC
{
    public class RiskScoreModel
    {
        public int UserId { get; set; }
        public int TotalScore { get; set; }
        public List<RiskScoreSectionScoreModel> SectionScores { get; set; }
        public RiskStatusModel Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

}
