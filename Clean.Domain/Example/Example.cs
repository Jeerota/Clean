using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.Example
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