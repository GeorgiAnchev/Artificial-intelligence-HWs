using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3
{
    class Program
    {
        public static List<string> Attributes = new List<string>() 
        { "Age", "Menopause", "TumorSize", "InvNodes", "NodeCaps", "DegMalig", "Breast", "BreastQuad", "Irradiat" };

        static void Main()
        {
            var records = LoadData();

            int crossValidationSetsCount = 277/5;
            int totalCount = records.Count();
            int testingSetCount = totalCount / crossValidationSetsCount;
            double totalAccuracy = 0;

            for (int i = 0; i < crossValidationSetsCount; i++)
            {
                var testSet = records.Skip(i * testingSetCount).Take(testingSetCount);
                var trainingSet = records.Take(i * testingSetCount).Concat(records.Skip((i+1) * testingSetCount).Take((crossValidationSetsCount - i) * testingSetCount));
                TreeNode root = CreateNode(trainingSet, Attributes, GetdominatingClass(trainingSet));//the dominatingOutcome should not be used in the first children 
                
                double succesfullPredictions = 0;
                foreach (var testRecord in testSet)
                {
                    OutcomeClass predictedOutcome = PredictOutcome(root, testRecord);
                    if (predictedOutcome == testRecord.OutcomeClass)
                    {
                        succesfullPredictions++;
                    }
                }
                Console.WriteLine($"For test set {i} success is {succesfullPredictions / testingSetCount * 100}%");
                totalAccuracy += succesfullPredictions / testingSetCount ;
            }

            Console.WriteLine($"The total accuracy is {totalAccuracy / crossValidationSetsCount * 100}%");

        }

        private static OutcomeClass PredictOutcome(TreeNode root, Record record)
        {
            var currentNode = root;
            while (!currentNode.IsLeaf)
            {
                string attrName = currentNode.NextAttributeName;
                string attrValue = typeof(Record).GetProperty(attrName).GetValue(record) as string;
                var nextNode = currentNode.Children.FirstOrDefault(node => node.AttributeValue == attrValue);
                if (nextNode == null)
                {
                    return (currentNode as TreeNode).DominatingParrentOutcome;
                }
                currentNode = nextNode;
            }
            return (OutcomeClass)currentNode.Outcome;
        }

        private static OutcomeClass GetdominatingClass(IEnumerable<Record> records)
        {
             return records.GroupBy(record => record.OutcomeClass)
                           .OrderByDescending(group => group.Count())
                           .First()
                           .First()
                           .OutcomeClass;
        }

        private static TreeNode CreateNode(IEnumerable<Record> records, IEnumerable<string> attributes, OutcomeClass dominatingParentOutcome)
        {
            var treeNode = new TreeNode();
            treeNode.DominatingParrentOutcome = dominatingParentOutcome;

            double minEntropy = 1000;
            string chosenAttribute = "";

            if (records.Count() == 0)//there are no examples in the subset, which happens when no example in the parent set was found to match a specific value of the selected attribute.Then a leaf node is created and labelled with the most common class of the examples in the parent node's set.
            {
                treeNode.IsLeaf = true;
                treeNode.Outcome = dominatingParentOutcome;
                return treeNode;
            }

            if (attributes.Count()==0)//there are no more attributes to be selected, but the examples still do not belong to the same class. In this case, the node is made a leaf node and labelled with the most common class of the examples in the subset.
            {
                var dominatingOutcome = GetdominatingClass(records);
                treeNode.IsLeaf = true;
                treeNode.Outcome = dominatingOutcome;
                return treeNode;
            }

            foreach (var attribute in attributes)
            {
                double entropy = getEntropy(records, attribute);
                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    chosenAttribute = attribute;
                }
            }
            
            treeNode.NextAttributeName = chosenAttribute;

            var property = typeof(Record).GetProperties().First(prop => prop.Name == chosenAttribute);

            var grouped = records.GroupBy(
                record => property.GetValue(record)
                );

            foreach (var group in grouped)
            {
                var childNode = new TreeNode();
                if (getEntropy(group, chosenAttribute) == 0)
                {
                    childNode.IsLeaf = true;
                    childNode.Outcome = group.First().OutcomeClass;
                    childNode.AttributeValue = group.Key as string;
                }
                else
                {
                    childNode = CreateNode(group, attributes.Where(attr => attr != chosenAttribute), GetdominatingClass(records));
                    childNode.AttributeValue = group.Key as string; //property.GetValue(group.First()) as string;
                }

                treeNode.Children.Add(childNode);
            }


            return treeNode;
        }

        private static double getEntropy(IEnumerable<Record> records, string attribute)
        {
            double entropy = 0;
            double totalCount = records.Count();

            var property = typeof(Record).GetProperties().First(prop => prop.Name == attribute);

            var grouped = records.GroupBy(
                record => property.GetValue(record)
                );

            foreach (var group in grouped)
            {
                double groupCount = group.Count();
                double first = group.Count(record => record.OutcomeClass == OutcomeClass.NoRecurrence);
                double second = group.Count(record => record.OutcomeClass == OutcomeClass.Recurrence);
                entropy += (groupCount / totalCount) * calculateEntropy(first, second);
            }

            return entropy;
        }

        public static double calculateEntropy(double a, double b)
        {
            if (a == 0 || b ==0)
            {
                return 0;
            }
            double total = a + b;
            double pa = a / total;
            double pb = b / total;
            double entropy = -((pa * Math.Log(pa, 2)) + (pb * Math.Log(pb, 2)));
            return entropy;
        }

        private static IEnumerable<Record> LoadData()
        {
            string text = System.IO.File.ReadAllText(@"C:\Users\Owner\Desktop\uni\is\HW\ID3\breast-cancer.data");
            var rows = text.Split(
                    new[] { "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                    );

            return rows.Select(row => new Record(row)).ToList().Where(record => !record.Invalid);
        }
    }
}
