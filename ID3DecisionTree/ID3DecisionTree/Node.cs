using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class Node {
        Attribute attribute { get; set; }

        Dictionary<string, Node> children { get; set; }

        public Node() {

        }

        public Node(Attribute best) {
            attribute = best;
            children = new Dictionary<string, Node>();
        }

        public void AddChild(string value, Node node) {
            if(children.ContainsKey(value)) {
                children[value] = node;
            } else {
                children.Add(value, node);
            }
        }

        public virtual string Choose(DataEntry example) {
            return PickChild(example).Choose(example);
        }

        public Node PickChild(DataEntry example) {
            return children[example.AttributeValues[this.attribute.Name]];
        }

        public virtual int Size() {
            return children.Select(x => x.Value.Size()).Aggregate(1, (x, y) => x + y);
        }

        public virtual string Display(int indentationLevel = 0) {
            var s = string.Empty;
            foreach(var kvp in this.children) {
                s = string.Format("{0}\n{1}{2} = {3}: {4}", s, new string(' ', indentationLevel * 3), attribute.Name, kvp.Key, kvp.Value.Display(indentationLevel + 1), indentationLevel);
            }
            return s;
        }

        public bool IsEndNode() {
            return children.Values.All(x => x.IsLeaf());
        }

        public virtual bool IsLeaf() {
            return false;
        }

        public List<DataEntry> ClassifyTestData(ID3DecisionTreeSchema id3TestSchema) {
            var misclassifications = new List<DataEntry>();
            foreach(var dataEntry in id3TestSchema.DataEntries) {
                if(Choose(dataEntry) != dataEntry.Class) {
                    misclassifications.Add(dataEntry);
                }

            }


            return misclassifications;
        }
    }

}
