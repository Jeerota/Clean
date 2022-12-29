using Microsoft.SqlServer.Dac.Model;
using System.Data;

namespace Clean.Generator
{
    public static class SQLConvertorHelper
    {
        public static Type GetCLRType(SqlDataType sqlType)
        {
            return sqlType switch
            {
                SqlDataType.BigInt => typeof(long),
                SqlDataType.Binary or SqlDataType.Image or SqlDataType.Timestamp or SqlDataType.VarBinary => typeof(byte[]),
                SqlDataType.Bit => typeof(bool),
                SqlDataType.Char or SqlDataType.NChar or SqlDataType.NText or SqlDataType.NVarChar or SqlDataType.Text or SqlDataType.VarChar or SqlDataType.Xml => typeof(string),
                SqlDataType.DateTime or SqlDataType.SmallDateTime or SqlDataType.Date or SqlDataType.Time or SqlDataType.DateTime2 => typeof(DateTime),
                SqlDataType.Decimal or SqlDataType.Money or SqlDataType.SmallMoney => typeof(decimal),
                SqlDataType.Float => typeof(double),
                SqlDataType.Int => typeof(int),
                SqlDataType.Real => typeof(float),
                SqlDataType.UniqueIdentifier => typeof(Guid),
                SqlDataType.SmallInt => typeof(short),
                SqlDataType.TinyInt => typeof(byte),
                SqlDataType.Variant => typeof(object),
                SqlDataType.Table => typeof(DataTable),
                SqlDataType.DateTimeOffset => typeof(DateTimeOffset),
                _ => throw new ArgumentOutOfRangeException(nameof(sqlType)),
            };
        }
    }
}
