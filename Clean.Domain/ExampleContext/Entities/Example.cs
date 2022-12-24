using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Examples")]
    public class Example : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }

        [ForeignKey("ExampleId")]
        public virtual ICollection<Sample>? Samples { get; set; }

        public ResultResponse Validate()
        {
            ResultResponse result = new();

            if (string.IsNullOrWhiteSpace(Name))
                result.Errors.Add(ValidationType.IsBlank.Message("Name"));

            return result;
        }
    }
}