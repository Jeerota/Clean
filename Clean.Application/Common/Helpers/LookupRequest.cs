using Clean.Application.Common.Enums;

namespace Clean.Application.Common.Helpers
{
    public abstract class LookupRequest
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public FilterType FilterType { get; set; } = FilterType.Inclusive;
        public CompareType CompareType { get; set; } = CompareType.All;
    }
}
