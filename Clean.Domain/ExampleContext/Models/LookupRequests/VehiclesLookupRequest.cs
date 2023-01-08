//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Model;

namespace Clean.Domain.ExampleContext.Models.LookupRequests
{
    public class VehiclesLookupRequest : LookupRequest
    {
		public System.Int64? Id { get; set; }
		public System.Int64? LocationId { get; set; }
		public System.String? Name { get; set; }
		public System.String? Make { get; set; }
		public System.String? Model { get; set; }
		public System.String? Plate { get; set; }
		public System.String? VIN { get; set; }

    }
}