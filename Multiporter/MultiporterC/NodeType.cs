using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class NodeType
    {
        public string Name { get; set; }
        public ExperimentNode Member { get; set; }
        public string Description { get; set; }

        public NodeType(string name, ExperimentNode m, string description)
        {
            Name = name;
            Member = m;
            Description = description;
            Member.Description = description;
        }
    }
}
