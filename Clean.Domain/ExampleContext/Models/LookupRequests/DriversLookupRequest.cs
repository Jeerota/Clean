//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Model;

namespace Clean.Domain.ExampleContext.Models.LookupRequests
{
    public class DriversLookupRequest : LookupRequest
    {
		public System.Int64? Id { get; set; }
		public System.Int64? LocationId { get; set; }
		public System.Int64? VehicleId { get; set; }
		public System.String? FirstName { get; set; }
		public System.String? LastName { get; set; }

    }
}