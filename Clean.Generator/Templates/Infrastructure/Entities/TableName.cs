//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Infrastructure.ContextNameContext.Entities
{
    public class TableName : BaseEntity
    {
        //Columns
        //ForeignKeys

        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            return other.GetType() != this.GetType()
                || !Update((TableName)other);
        }

        private bool Update(TableName other)
        {
            bool result = false;
            //CompareColumns
            return result;
        }
    }
}