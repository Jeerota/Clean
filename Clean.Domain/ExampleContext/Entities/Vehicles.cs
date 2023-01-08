//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Vehicles")]
    public class Vehicles : BaseEntity
    {
		public System.Int64? Id { get; set; }
		[ForeignKey("Locations")]
		public System.Int64 LocationId { get; set; }
		public System.String? Name { get; set; }
		public System.String? Make { get; set; }
		public System.String? Model { get; set; }
		public System.String? Plate { get; set; }
		public System.String? VIN { get; set; }

		public virtual ICollection<Locations> Locations { get; set; }

		[ForeignKey("VehicleId")]
		public virtual ICollection<Drivers> Drivers { get; set; }

        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}