using ID3DecisionTree.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3DecisionTree {
    class Program {
        static void Main(string[] args) {
            if (args.Length >= 2) {
                var id3TrainSchema = new FileParser(args[0]).DecisionTreeSchema;
                var id3TestSchema = new FileParser(args[1]).DecisionTreeSchema;
                var factory = new ID3DecisionTreeFactory(id3TrainSchema);
                var tree = factory.BuildTree(id3TrainSchema.DataEntries, id3TrainSchema.Attributes);
                var misclassifications = tree.ClassifyTestData(id3TestSchema);

                Console.WriteLine("Decision Tree:");
                Console.WriteLine(tree.Display());
                Console.WriteLine();
                Console.WriteLine(string.Format("Misclassifications Found: {2}% ({0}/{1})", misclassifications.Count, id3TestSchema.DataEntries.Count, (decimal)misclassifications.Count / (decimal)id3TestSchema.DataEntries.Count));

                foreach(var misclassification in misclassifications) {
                    Console.WriteLine(misclassification.Name);
                }

                Console.ReadKey();
            } else {
                Console.WriteLine("Command Line Arguments Error. The first argument should be the training data file, and the second argument the test data file");
            }
            
        }
    }
}
