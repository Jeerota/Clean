using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ExampleContext.Entities
{
    [Table("Samples")]
    public class Sample : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Example")]
        public long ExampleId { get; set; }
        public string? Name { get; set; }

        public virtual Example? Example { get; set; }

        public ResultResponse Validate()
        {
            ResultResponse result = new();

            if (string.IsNullOrWhiteSpace(Name))
                result.Errors.Add(ValidationType.IsBlank.Message("Name"));

            return result;
        }
    }
}