using Clean.Domain.Common.Model;

namespace Clean.Domain.ExampleContext.Models.LookupRequests
{
    public class SampleLookupRequest : LookupRequest
    {
        public long? Id { get; set; }
        public long? ExampleId { get; set; }
        public string? Name { get; set; }
    }
}