using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Domain.ContextNameContext.Entities
{
    [Table("TableName")]
    public class TableName : BaseEntity
    {
//Columns

        public ResultResponse Validate()
        {
            ResultResponse result = new();
            return result;
        }
    }
}