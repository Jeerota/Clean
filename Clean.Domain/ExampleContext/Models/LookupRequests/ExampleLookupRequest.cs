using Clean.Domain.Common.Model;

namespace Clean.Domain.ExampleContext.Models.LookupRequests
{
    public class ExampleLookupRequest : LookupRequest
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
    }
}