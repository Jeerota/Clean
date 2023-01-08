//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Locations")]
    public class Locations : BaseEntity
    {
		public System.Int64? Id { get; set; }
		public System.String? Name { get; set; }
		public System.String AddressLine1 { get; set; }
		public System.String? AddressLine2 { get; set; }
		public System.String City { get; set; }
		public System.String? State { get; set; }
		public System.String Country { get; set; }
		public System.String? Zip { get; set; }


		[ForeignKey("LocationId")]
		public virtual ICollection<Drivers> Drivers { get; set; }
		[ForeignKey("LocationId")]
		public virtual ICollection<Vehicles> Vehicles { get; set; }

        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}