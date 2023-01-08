//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Model;

namespace Clean.Domain.ExampleContext.Models.LookupRequests
{
    public class LocationsLookupRequest : LookupRequest
    {
		public System.Int64? Id { get; set; }
		public System.String? Name { get; set; }
		public System.String? AddressLine1 { get; set; }
		public System.String? AddressLine2 { get; set; }
		public System.String? City { get; set; }
		public System.String? State { get; set; }
		public System.String? Country { get; set; }
		public System.String? Zip { get; set; }

    }
}