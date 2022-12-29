using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Generator.Models
{
    public class ForeignKey
    {
        public string Name { get; set; }
        public string ForeignTable { get; set; }
        public List<string> ForeignColumns { get; set; }
        public string DefiningTable { get; set; }
        public List<string> DefiningColumns { get; set; }

        public ForeignKey(string name)
        {
            Name = name;
            ForeignColumns = new();
            DefiningColumns = new();
        }
    }
}
