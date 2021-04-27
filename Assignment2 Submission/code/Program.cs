using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {

    class Program {

        static void Main(string[] args) {

            bool? t = true;
            bool? f = false;
            bool? n = null;

            bool? TT = !(t ^ t);
            bool? TF = !(f ^ t);
            bool? FF = !(f ^ f);
            bool? TN = !(n ^ t);
            bool? FN = !(f ^ n);

            // Check if the file exists
            if (!FileHandler.FilePathwayExists(args[1])) {

                Console.WriteLine("File doesn't exist, check pathway and try again.");
            }
            else {

                // Open the file and parse contents into a KnowledgeBase and a Query
                FileHandler.ParseFile();
                KnowledgeBase KB = new KnowledgeBase(FileHandler.Clauses);
                Clause query = Clause.GetClause(FileHandler.Query);

                // Execute the appropriate search method with the KnowledgeBase and Query from the file
                switch (args[0].ToUpper()) {

                    case "TEST": {
                            Console.WriteLine($"Testing file: {args[1]}");
                            foreach (string clause in FileHandler.Clauses) {
                                Console.WriteLine(clause);
                            }
                            Console.WriteLine($"Search for {FileHandler.Query}");

                            Console.WriteLine("\nResults:");
                            Console.WriteLine($"Truth Table - {new TruthTable().Search(KB, query)}");
                            Console.WriteLine($"Forward Chain - {new ForwardChaining().Search(KB, query)}");
                            Console.WriteLine($"Backward Chain - {new BackwardChaining().Search(KB, query)}");
                            Console.WriteLine($"Resolution Chain - {new ResolutionForwardChaining().Search(KB, query)}");
                            break;
                        }

                    case "TT": {
                            Console.WriteLine(new TruthTable().Search(KB, query));
                            break;
                        }

                    case "FC": {
                            Console.WriteLine(new ForwardChaining().Search(KB, query));
                            break;
                        }

                    case "BC": {
                            Console.WriteLine(new BackwardChaining().Search(KB, query));
                            break;
                        }

                    case "RFC": {
                            Console.WriteLine(new ResolutionForwardChaining().Search(KB, query));
                            break;
                        }
                }
            }

            Console.ReadLine();
        }
    }
}
