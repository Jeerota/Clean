namespace Clean.Generator.Models
{
    public class Table
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }

        public bool IsKeyless 
        { 
            get
            {
                return this.Columns == null
                    || this.Columns.All(column => !column.IsPrimaryKey);
            } 
        }
        
        public Table(string name, string schema = "dbo")
        {
            Schema = schema;
            Name = name;
            Columns = new();
            ForeignKeys = new();
        }
    }
}
