using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class ID3DecisionTreeSchema {
        public TargetAttribute TargetAttribute { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public List<DataEntry> DataEntries { get; private set; }

        public ID3DecisionTreeSchema(TargetAttribute targetAttribute, List<Attribute> attributes, List<DataEntry> dataEntries) {
            TargetAttribute = targetAttribute;
            Attributes = attributes;
            DataEntries = dataEntries;
        }

        public override string ToString() {
            var attributes = Attributes.Select(x => x.ToString()).ToArray();
            var datas = DataEntries.Select(x => x.ToString()).ToArray();

            return string.Format("[ID3DecisionTreeSchema: Attributes\n{0}\nTargetAttribute{1}\nData\n{2}\n]", string.Join("\n", attributes), TargetAttribute, string.Join("\n", datas)
            );
        }

        public Tuple<List<DataEntry>, List<DataEntry>> SplitRandomly() {
            List<DataEntry> clone = DataEntries.ToList();

            //Shuffle the list of entries
            var rng = new Random();
            var n = clone.Count;
            while(n > 1) {
                n--;
                int k = rng.Next(n + 1);
                var value = clone[k];
                clone[k] = clone[n];
                clone[n] = value;
            }

            var middle = (clone.Count) / 2;
            var remainder = clone.Count % 2;
            return new Tuple<List<DataEntry>, List<DataEntry>>(clone.Take(middle).ToList(), clone.Skip(middle).Take(middle + remainder).ToList());
        }
    }
}
