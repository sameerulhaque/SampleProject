namespace SampleProject.Shared.Models.KYC
{
    public class UserSubmissionModel
    {
        public int UserId { get; set; }
        public int ConfigId { get; set; }
        public int CompanyId { get; set; }
        public List<UserSubmissionSectionModel> Sections { get; set; }
    }

}
