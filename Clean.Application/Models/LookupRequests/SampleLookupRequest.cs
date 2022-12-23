using Clean.Application.Common.Helpers;

namespace Clean.Application.Models.LookupRequests
{
    public class SampleLookupRequest : LookupRequest
    {
        public long? Id { get; set; }
        public long? ExampleId { get; set; }
        public string? Name { get; set; }
    }
}