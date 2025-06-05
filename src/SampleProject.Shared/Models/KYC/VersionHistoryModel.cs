namespace SampleProject.Shared.Models.KYC
{
    public class VersionHistoryModel
    {
        public int Id { get; set; }
        public string EntityType { get; set; } // Consider enum: Configuration, Submission
        public int EntityId { get; set; }
        public string Version { get; set; }
        public string Timestamp { get; set; }
        public string Changes { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

}
