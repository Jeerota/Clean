//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//
//Last Generated: 12/29/2022 03:37//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Samples")]
    public class Samples : BaseEntity
    {
		public System.Int64? Id { get; set; }
		[ForeignKey("Examples")]
		public System.Int64 ExampleId { get; set; }
		public System.String? Name { get; set; }

		public virtual ICollection<Examples> Examples { get; set; }


        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}