using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.Example
{
    [Table("Samples")]
    public class Sample : SystemFields
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Example")]
        public long ExampleId { get; set; }
        public string? Name { get; set; }

        public virtual Example? Example { get; set; }
    }
}