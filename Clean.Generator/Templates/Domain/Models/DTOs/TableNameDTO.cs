//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Entities;
using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Models;

namespace Clean.Domain.ContextNameContext.Models.DTOs
{
    public class TableNameDTO
    {
//Columns
//ForeignKeys
        public ResultResponse<TableNameDTO> Validate(bool isUpdate = false)
        {
            ResultResponse<TableNameDTO> result = new();
            result.Errors.Add("");
//ValidateColumns
            return result;
        }
    }
}