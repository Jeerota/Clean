namespace Clean.Domain.Common.Models
{
    public class FetchResponse<T> where T : class
    {
        public int TotalRecords { get; set; }
        public int RecordCount
        {
            get
            {
                return Entities?.Count() ?? 0;
            }
        }

        public int PageLimit { get; set; }
        public int Page { get; set; }
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalRecords / (decimal)PageLimit);
            }
        }

        public IEnumerable<T>? Entities { get; set; }

        public FetchResponse() { }
    }
}