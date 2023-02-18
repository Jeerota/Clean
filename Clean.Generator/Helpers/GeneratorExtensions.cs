using Clean.Generator.Models;
using System.Text.RegularExpressions;

namespace Clean.Generator.Helpers
{
    public static class GeneratorExtensions
    {
        public static string GetTableName(bool includeSchemaLabel, Table table)
        {
            return includeSchemaLabel
                ? (table.Schema.ToLower() + "_") + table.Name
                : table.Name;
        }

        public static string GetTableName(bool includeSchemaLabel, ForeignKey foreignKey, bool definitionTable = false)
        {
            return includeSchemaLabel
                ? definitionTable 
                    ? (foreignKey.DefiningSchema.ToLower() + "_") + foreignKey.DefiningTable
                    : (foreignKey.ForeignSchema.ToLower() + "_") + foreignKey.ForeignTable
                : definitionTable
                    ? foreignKey.DefiningTable
                    : foreignKey.ForeignTable;
        }

        public static string GetPropertyNameFromColumnName(Table table, string columnName)
        {
            Regex rgx = new Regex("[^A-Za-z0-9_]");
            return table.Name == columnName
                ? rgx.Replace(columnName, "") + "_Column"
                : rgx.Replace(columnName, "");
        }
    }
}
