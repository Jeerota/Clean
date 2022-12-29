using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Generator.Models
{
    public class Table
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }
        
        public Table(string name, string schema = "dbo")
        {
            Schema = schema;
            Name = name;
            Columns = new();
            ForeignKeys = new();
        }
    }
}
