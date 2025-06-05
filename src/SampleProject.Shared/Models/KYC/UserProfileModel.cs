namespace SampleProject.Shared.Models.KYC
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? RiskScore { get; set; }
        public RiskStatusModel Status { get; set; }
        public string SubmissionDate { get; set; }
    }

}
