using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class Attribute {
        public int Index { get; private set; }

        public string Name { get; private set; }

        public List<string> Values { get; private set; }

        public Attribute(int id, string name, params string[] types) {
            Index = id;
            Name = name;
            Values = types.ToList();
        }

        public override string ToString() {
            return string.Format("[Attribute: Index={0}, Name={1}, Type={2}]", Index, Name, string.Join(",", Values));
        }
    }
}
