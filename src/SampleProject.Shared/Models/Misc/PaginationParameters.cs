using System.Linq.Expressions;

namespace SampleProject.Shared.Models.Misc
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public string SortColumn { get; set; } = "Id";
        public string SortDirection { get; set; } = "asc";
        public Dictionary<string, string> SearchQuery { get; set; } = new Dictionary<string, string>();
        public List<string> IncludeQuery { get; set; } = new List<string>();
    }

}
