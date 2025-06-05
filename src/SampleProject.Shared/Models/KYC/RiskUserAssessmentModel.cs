namespace SampleProject.Shared.Models.KYC
{
    public class RiskUserAssessmentModel
    {
        public int UserId { get; set; }
        public int ConfigId { get; set; }
        public List<RiskUserAssessmentSectionModel> Sections { get; set; }
        public List<RiskUserAssessmentSectionScoreModel>? SectionScores { get; set; }
        public int? TotalScore { get; set; }
        public RiskStatusModel? Status { get; set; }
    }

}
