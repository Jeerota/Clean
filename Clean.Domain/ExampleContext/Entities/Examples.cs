//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Examples")]
    public class Examples : BaseEntity
    {
		public System.Int64? Id { get; set; }
		public System.String? Name { get; set; }


		[ForeignKey("ExampleId")]
		public virtual ICollection<Samples> Samples { get; set; }

        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}