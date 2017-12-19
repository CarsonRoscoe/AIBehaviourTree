using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class TargetAttribute : Attribute {
        public TargetAttribute(int id, string name, params string[] types) : base(id, name, types) {
        }
    }
}
