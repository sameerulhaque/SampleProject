namespace SampleProject.Shared.Models.Misc
{
    public class PagedList<T>(int pageSize, int pageNumber, int totalCount, IReadOnlyList<T> data)
    {
        public int PageSize { get; set; } = pageSize;
        public int PageNumber { get; set; } = pageNumber;
        public int TotalCount { get; set; } = totalCount;
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public IEnumerable<T> Data { get; set; } = data;
        public static PagedList<T> Create(int pageSize, int pageNumber, int totalCount, IReadOnlyList<T> data)
        {
            return new PagedList<T>(pageSize, pageNumber, totalCount, data);
        }
    }

}
