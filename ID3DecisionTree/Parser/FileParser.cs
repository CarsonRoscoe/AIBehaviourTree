using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ID3DecisionTree.Parser {
    class FileParser {
        static readonly Regex trimmer = new Regex(@"\s\s+");

        public ID3DecisionTreeSchema DecisionTreeSchema { get; private set; }

        enum Expecting { ClassOrFeatureCount, Features, ExampleCount, Examples, Nothing }

        public FileParser(string fileName) {

            using(var streamReader = new StreamReader(fileName)) {
                string line = string.Empty;
                List<string> classValues = new List<string>();
                int numberOfFeatures = -1;
                List<Attribute> features = new List<Attribute>();
                int numberOfExamples = -1;
                List<DataEntry> examples = new List<DataEntry>();
                TargetAttribute targetAttribute = null;

                int featureCounter = 0;
                int exampleCounter = 0;

                var expecting = Expecting.ClassOrFeatureCount;

                do {
                    line = streamReader.ReadLine();

                    //If it's empty, skip it
                    if (line.Length > 0) {
                        //If it's a comment, skip it
                        if (line.Length > 2 && line[0] == '/' && line[1] == '/') {
                            continue;
                        }

                        //Valid line. Where the magic happens!
                        switch(expecting) { 
                            case Expecting.ClassOrFeatureCount:
                                //If we're a number, set the FeatureCounter and change states
                                if (int.TryParse(line, out numberOfFeatures)) {
                                    expecting = Expecting.Features;
                                    targetAttribute = new TargetAttribute(0, "class", classValues.ToArray());
                                } else {
                                    //Otherwise, we're a class that needs to be read
                                    classValues.Add(line);
                                }
                                break;
                            case Expecting.Features:
                                var featureParts = line.Split(' ');
                                features.Add(new Attribute(featureCounter++ + 1, featureParts[0], featureParts.Skip(1).ToArray()));
                                if (featureCounter >= numberOfFeatures) {
                                    expecting = Expecting.ExampleCount;
                                }
                                break;
                            case Expecting.ExampleCount:
                                if (int.TryParse(line, out numberOfExamples)) {
                                    expecting = Expecting.Examples;
                                }
                                break;
                            case Expecting.Examples:
                                line = trimmer.Replace(line, " ");
                                var exampleParts = line.Split(null);
                                if (exampleParts.Length == 2 + featureCounter) {
                                    var entryName = exampleParts[0];
                                    var classValue = exampleParts[1];
                                    var attributeValues = exampleParts.Skip(2).ToList();
                                    if (attributeValues.Count == features.Count) {
                                        var attribVals = new Dictionary<string, string>();
                                        for(int i = 0; i < attributeValues.Count; i++) {
                                            attribVals[features[i].Name] = attributeValues[i];
                                        }
                                        var dataEntry = new DataEntry(classValue, attribVals, entryName);
                                        examples.Add(dataEntry);
                                    }
                                    if(++exampleCounter >= numberOfExamples) {
                                        expecting = Expecting.Nothing;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                } while(!streamReader.EndOfStream);
                DecisionTreeSchema = new ID3DecisionTreeSchema(targetAttribute, features, examples);
            }
        }
    }
}
