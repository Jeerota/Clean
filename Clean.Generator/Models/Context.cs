using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Generator.Models
{
    public class Context
    {
        public string Name { get; set; }
        public List<Table> Tables { get; set; }

        public Context(string name)
        {
            Name = name;
            Tables = new();
        }
    }
}
