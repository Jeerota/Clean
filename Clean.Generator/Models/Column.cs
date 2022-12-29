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
        public Type DataType { get; set; }
        public bool Nullable { get; set; }

        public Column(string name)
        {
            Name = name;
        }
    }
}
