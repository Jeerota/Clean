using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Generator.Models
{
    public class Column
    {
        public string Name { get; set; }
        public List<string> Properties { get; set; }
        public List<string> Metadata { get; set; }

        public Column(string name)
        {
            Name = name;
            Properties = new();
            Metadata = new();
        }
    }
}
