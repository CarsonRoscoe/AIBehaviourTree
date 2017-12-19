using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class LeafNode : Node {
        string targetAttributesValue { get; set; }

        public LeafNode(string decision) {
            targetAttributesValue = decision;
        }

        public override string Choose(DataEntry example) {
            return targetAttributesValue;
        }

        public override int Size() {
            return 1;
        }

        public override string Display(int depth) {
            return targetAttributesValue;
        }

        public override bool IsLeaf() {
            return true;
        }
    }
}
