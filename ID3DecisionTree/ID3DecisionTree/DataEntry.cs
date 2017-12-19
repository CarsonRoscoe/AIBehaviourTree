using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class DataEntry {
        public string Name { get; private set; }
        public string Class { get; private set; }
        public Dictionary<string, string> AttributeValues { get; private set; }
        public DataEntry(string classValue, Dictionary<string, string> attributeValues, string entryName) {
            Class = classValue;
            AttributeValues = attributeValues;
            Name = entryName;
        }

        public override string ToString() {
            return string.Format("[DataEntry: Class={0}, Values={1}]", Class, string.Join(",", AttributeValues.Select(x => x))
            );
        }

        public override int GetHashCode() {
            return AttributeValues.GetHashCode();
        }
    }
}
