using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {

    public class TruthTable : SearchMethod {

        private int validModels = 0;

        public override string Search (KnowledgeBase KB, Clause query) {

            // Initialise variables
            // Get one of each symbol, with a positive polarity, to initialise the model
            List<Symbol> symbols = new List<Symbol>();
            foreach (Symbol s in Clause.GetAll<Symbol>()) {
                if (!symbols.Contains(s.polarity ? s : ~s)) {
                    symbols.Add(s.polarity ? s : ~s);
                }
            }

            KnowledgeBase model = new KnowledgeBase(new List<string>());
            
            // Begin recursive check
            if (CheckAll(KB, query, symbols, model)) {
                return "YES: " + validModels.ToString();
            }

            return "NO";
        }

        private bool CheckAll (KnowledgeBase origKB, Clause query, List<Symbol> symbols, KnowledgeBase model) {

            // If symbols is empty
            if (symbols.Count == 0) {
                //Console.WriteLine("Empty: " + string.Join(", ", model.clauses.Values.Select(a => a.literal)));

                // if PL(KB, model) == True
                if (origKB.Entails(model)) {

                    // return PL(query, model)
                    validModels++;
                    return (query.Evaluate(model) == true);
                }
                
                else {
                    return true;
                }
            }
            else {
                //Console.WriteLine("Not Empty: " + string.Join(", ", model.clauses.Values.Select(a => a.literal)));

                // p = take first symbol out of symbols, symbols now only contains the leftovers
                Symbol p = symbols[0];
                symbols.RemoveAt(0);
                
                // Begin next level of recursive checks
                return (CheckAll(origKB, query, new List<Symbol>(symbols), model.Extend(p)) && CheckAll(origKB, query, new List<Symbol>(symbols), model.Extend(~p)));
            }
        }
    }
}
