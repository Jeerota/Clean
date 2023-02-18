using Microsoft.SqlServer.Dac.Model;

namespace Clean.Generator.Models
{
    public class ForeignKey
    {
        public string Name { get; set; }
        public string? ForeignSchema { get; set; }
        public string? ForeignTable { get; set; }
        public string RelationshipForeign { get; set; }
        public List<string> ForeignColumns { get; set; }
        public string? DefiningSchema { get; set; }
        public string? DefiningTable { get; set; }
        public string RelationshipDefining { get; set; }
        public List<string> DefiningColumns { get; set; }
        public ForeignKeyAction Delete { get; set; }
        public ForeignKeyAction Update { get; set; }

        public ForeignKey(string name)
        {
            Name = name;
            ForeignColumns = new();
            DefiningColumns = new();
        }

        public bool Validate()
        {
            if (ForeignTable == null)
                return false;
            if (DefiningTable == null)
                return false;
            return true;
        }
    }
}
