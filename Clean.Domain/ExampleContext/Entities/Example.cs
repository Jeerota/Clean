using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clean.Domain.Common;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Examples")]
    public class Example : SystemFields
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }

        [ForeignKey("ExampleId")]
        public virtual ICollection<Sample>? Samples { get; set; }
    }
}