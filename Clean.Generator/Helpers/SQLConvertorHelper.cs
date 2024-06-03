using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Dac.Model;
using System.Data;

namespace Clean.Generator.Helpers
{
    public static class SQLConvertorHelper
    {
        public static DeleteBehavior GetDeleteBehavior(ForeignKeyAction action)
        {
            return action switch
            {
                ForeignKeyAction.SetNull => DeleteBehavior.ClientSetNull,
                ForeignKeyAction.Cascade => DeleteBehavior.ClientCascade,
                ForeignKeyAction.SetDefault => DeleteBehavior.Restrict,
                _ => throw new ArgumentOutOfRangeException(nameof(action)),
            };
        }

        public static string GetCLRType(SqlDataType sqlType)
        {
            return sqlType switch
            {
                SqlDataType.TinyInt => "byte",
                SqlDataType.SmallInt => "short",
                SqlDataType.Int => "int",
                SqlDataType.BigInt => "long",
                SqlDataType.Binary or SqlDataType.Image or SqlDataType.Timestamp or SqlDataType.VarBinary => "byte[]",
                SqlDataType.Bit => "bool",
                SqlDataType.Char or SqlDataType.NChar or SqlDataType.NText or SqlDataType.NVarChar or SqlDataType.Text or SqlDataType.VarChar or SqlDataType.Xml => "string",
                SqlDataType.DateTime or SqlDataType.SmallDateTime or SqlDataType.Date or SqlDataType.Time or SqlDataType.DateTime2 => "DateTime",
                SqlDataType.Decimal or SqlDataType.Money or SqlDataType.SmallMoney or SqlDataType.Numeric => "decimal",
                SqlDataType.Float => "double",
                SqlDataType.Real => "float",
                SqlDataType.UniqueIdentifier => "Guid",
                SqlDataType.Variant => "object",
                SqlDataType.Table => "DataTable",
                SqlDataType.DateTimeOffset => "DateTimeOffset",
                SqlDataType.Rowversion => "DataRowVersion",
                SqlDataType.Unknown => "byte[]",
                _ => throw new ArgumentOutOfRangeException(nameof(sqlType)),
            };
        }

        public static dynamic? ConvertDefaultToDataType(string dataType, string defaultValue)
        {
            if(string.IsNullOrWhiteSpace(dataType)) 
                throw new ArgumentNullException(nameof(dataType));

            if (string.IsNullOrWhiteSpace(defaultValue))
                throw new ArgumentNullException(nameof(defaultValue));

            return dataType switch
            {
                "byte" => Convert.ToByte(defaultValue),
                "short" => Convert.ToInt16(defaultValue),
                "int" => Convert.ToInt32(defaultValue),
                "long" => Convert.ToInt64(defaultValue), 
                "bool" => Convert.ToBoolean(defaultValue),
                "string" => defaultValue,
                "DateTime" => Convert.ToDateTime(defaultValue),
                "decimal" => Convert.ToDecimal(defaultValue),
                "double" => Convert.ToDouble(defaultValue),
                "float" => float.TryParse(defaultValue, out float floatResult) ? floatResult : (float?)null,
                "Guid" => Guid.TryParse(defaultValue, out Guid guidResult) ? guidResult : (Guid?)null,
                "DateTimeOffset" => DateTimeOffset.TryParse(defaultValue, out DateTimeOffset dateTimeOffsetResult) ? dateTimeOffsetResult : (DateTimeOffset?)null,
                "DataRowVersion" => DataRowVersion.TryParse(defaultValue, out DataRowVersion dataRowVersionResult) ? dataRowVersionResult : (DataRowVersion?)null,
                ////Unsupported Default values as of 2/14/23
                //SqlDataType.Binary or SqlDataType.Image or SqlDataType.Timestamp or SqlDataType.VarBinary => "byte[]",
                //SqlDataType.Variant => "object",
                //SqlDataType.Table => "DataTable",
                //SqlDataType.Unknown => "byte[]",
                _ => throw new ArgumentOutOfRangeException(nameof(dataType)),
            };
        }
    }
}
