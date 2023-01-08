//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Drivers")]
    public class Drivers : BaseEntity
    {
		public System.Int64? Id { get; set; }
		[ForeignKey("Locations")]
		public System.Int64? LocationId { get; set; }
		[ForeignKey("Vehicles")]
		public System.Int64? VehicleId { get; set; }
		public System.String? FirstName { get; set; }
		public System.String? LastName { get; set; }

		public virtual ICollection<Locations> Locations { get; set; }
		public virtual ICollection<Vehicles> Vehicles { get; set; }


        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}