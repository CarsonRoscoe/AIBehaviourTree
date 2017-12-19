using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class ID3DecisionTreeFactory {
        private ID3DecisionTreeSchema treeSchema { get; set; }

        public ID3DecisionTreeFactory(ID3DecisionTreeSchema id3Schema) {
            treeSchema = id3Schema;
        }

        public Node BuildTree(List<DataEntry> examples, List<Attribute> attributes) {
            return DecisionTreeLearner(examples, treeSchema.Attributes, examples);
        }

        Node DecisionTreeLearner(List<DataEntry> examples, List<Attribute> attributes, List<DataEntry> parentExamples) {
            if(examples.Count == 0) {
                return new LeafNode(this.Plurality(parentExamples));
            } else if(this.IsClassificationSame(examples)) {
                return new LeafNode(examples[0].Class);
            } else if(attributes.Count == 0) {
                return new LeafNode(this.Plurality(examples));
            } else {
                var mostImportantAttribute = this.Importance(examples, attributes);

                var filteredAttributes = attributes.ToList();
                filteredAttributes.RemoveAll(x => x.Name == mostImportantAttribute.Name);

                var tree = new Node(mostImportantAttribute);
                var partitions = this.Partition(examples, mostImportantAttribute);
                var values = mostImportantAttribute.Values;

                foreach(var kvp in partitions) {
                    tree.AddChild(kvp.Key, this.DecisionTreeLearner(kvp.Value, filteredAttributes, examples));
                    if(values.Contains(kvp.Key)) {
                        values.Remove(kvp.Key);
                    }
                }

                foreach(var value in values) {
                    tree.AddChild(value, new LeafNode(this.Plurality(examples)));
                }

                return tree;
            }
        }

        Dictionary<string, List<DataEntry>> Partition(List<DataEntry> examples, Attribute best) {
            var partitions = new Dictionary<string, List<DataEntry>>();

            foreach(var example in examples) {
                var key = example.AttributeValues[best.Name];
                if(partitions.ContainsKey(key)) {
                    partitions[key].Add(example);
                } else {
                    partitions.Add(key, new List<DataEntry>() { example });
                }
            }

            return partitions;
        }

        bool IsClassificationSame(List<DataEntry> examples) {
            return examples.Select(x => x.Class).Distinct().Count() == 1;
        }

        string Plurality(List<DataEntry> examples) {
            var high = examples[0].Class;
            var counts = new Dictionary<string, int>() { { high, 0 } };
            foreach(var item in examples) {
                var label = item.Class;
                if(counts.ContainsKey(label)) {
                    counts[label]++;
                    if(counts[label] > counts[high]) {
                        high = label;
                    }
                } else {
                    counts.Add(label, 0);
                }
            }
            return high;
        }

        Attribute Importance(List<DataEntry> examples, List<Attribute> attributes) {
            var best = attributes[0];
            var bestChange = this.EntropyDifference(examples, best);
            foreach(var attribute in attributes) {
                var change = this.EntropyDifference(examples, attribute);
                if(change > bestChange) {
                    best = attribute;
                    bestChange = change;
                }
            }
            return best;
        }

        double EntropyDifference(List<DataEntry> examples, Attribute attribute) {
            return this.Entropy(examples) - this.SplitEntropy(examples, attribute);
        }

        double Entropy(List<DataEntry> examples) {
            var labelCounts = new Dictionary<string, int>();
            foreach(var example in examples) {
                if(labelCounts.ContainsKey(example.Class)) {
                    labelCounts[example.Class]++;
                } else {
                    labelCounts.Add(example.Class, 1);
                }
            }
            return this.Entropy(labelCounts, examples.Count);
        }

        double SplitEntropy(List<DataEntry> examples, Attribute attribute) {
            var partitions = this.Partition(examples, attribute);
            double entropy = 0;
            foreach(var kvp in partitions) {
                var splitentropy = this.Entropy(kvp.Value);
                entropy += (kvp.Value.Count / (double)(examples.Count)) * splitentropy;
            }
            return entropy;
        }

        double Entropy(Dictionary<string, int> labelCounts, int total) {
            double entropy = 0;
            double part = 0;
            foreach(var kvp in labelCounts) {
                var count = kvp.Value;
                double rate = count / (double)total;
                if(rate != 0) {
                    part = rate * Math.Log(rate, 2);
                }
                entropy += part;
            }
            return part * -1;
        }

        Tuple<int, int> SplitIntoPositiveNegative(List<DataEntry> examples, string trueCase = "survived") {
            var data = examples.GroupBy(x => x.Class).ToDictionary(g => g.Key.Equals(trueCase), g => g.ToList().Count());
            var pcount = data.Count > 0 && data.Any(x => x.Key) ? data.Where(x => x.Key).First().Value : 0;
            var ncount = data.Count > 0 && data.Any(x => !x.Key) ? data.Where(x => !x.Key).First().Value : 0;
            return new Tuple<int, int>(pcount, ncount);
        }
    }

}
