using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {

    /// <summary>
    /// A relational database describing the relationships between data for a given horn form knowledge base
    /// </summary>
    public class HornDatabase {

        /// <summary>
        /// A dictionary [clause, list[symbol]] indexed by clause, containing a list of symbols which that clause uses 
        /// </summary>
        public Dictionary<string, List<string>> clauseSymbols = new Dictionary<string, List<string>>();

        /// <summary>
        /// A dictionary [symbol, list[clauses]] indexed by symbol, containing a list of clauses which use that symbol
        /// </summary>
        public Dictionary<string, List<string>> symbolClauses = new Dictionary<string, List<string>>();

        /// <summary>
        /// A dictionary [clause, result] indexed by clause, containing the result of each clause
        /// </summary>
        public Dictionary<string, string> clauseResult = new Dictionary<string, string>();

        /// <summary>
        /// A dictionary [result, list[clause]] indexed by result, containing a list of clauses with that result
        /// </summary>
        public Dictionary<string, List<string>> resultClauses = new Dictionary<string, List<string>>();

        /// <summary>
        /// A dictionary [clause, count] indexed by clause, containing the number of symbols in that clause which have yet to be proven true
        /// </summary>
        public Dictionary<string, int> unresolvedCount = new Dictionary<string, int>();

        /// <summary>
        /// A list [facts] of the singular units of propositional logic that were found in the KB
        /// </summary>
        public List<string> facts = new List<string>();

        public HornDatabase (KnowledgeBase knowledgeBase) {

            foreach (Clause clause in knowledgeBase.clauses.Values) {

                // 1. RULES:
                // Set a counter for each rule
                if (clause is Rule rule) {

                    // Pair the result with this clause
                    UpdateResult(rule.identity, rule.consequent.identity);

                    // Iterate through all nested conjuncts of the clause
                    // Continue to get the consequent of a conjunction, until the clause is a symbol, not a rule
                    Clause ptr = rule.antecedent;
                    while (ptr is Rule nestedrule) {

                        // Pair the symbol at this position with the clause (the antecedent of the current ptr)
                        UpdateSymbol(rule.identity, nestedrule.antecedent.identity);

                        // Point to the consequent, and loop back up to check if it is another nested conjunction rule
                        ptr = nestedrule.consequent;
                    }

                    // Pair the final symbol with the clause
                    UpdateSymbol(rule.identity, ptr.identity);
                }

                // 2. SYMBOLS:
                // Add each known symbol to the list of facts
                else if (clause is Symbol symbol) {
                    facts.Add(symbol.identity);
                }
            }
        }

        /// <summary>
        /// Update all data structures to contain a relationship between this clause and symbol
        /// </summary>
        private void UpdateSymbol (string clause, string symbol) {

            // Add this symbol to a list for this clause
            if (!clauseSymbols.ContainsKey(clause)) {
                clauseSymbols.Add(clause, new List<string>());
            }
            clauseSymbols[clause].Add(symbol);

            // Add this clause to a list for this symbol
            if (!symbolClauses.ContainsKey(symbol)) {
                symbolClauses.Add(symbol, new List<string>());
            }
            symbolClauses[symbol].Add(clause);

            // Increment the symbol counter for this clause
            if (!unresolvedCount.ContainsKey(clause)) {
                unresolvedCount.Add(clause, 0);
            }
            unresolvedCount[clause]++;
        }

        /// <summary>
        /// Update all data structures to contain a relationship between this clause and result
        /// </summary>
        private void UpdateResult (string clause, string result) {

            // Add this result to a pair for this clause
            if (!clauseResult.ContainsKey(clause)) {
                clauseResult.Add(clause, "");
            }
            clauseResult[clause] = result;

            // Add this clause to a list for this result
            if (!resultClauses.ContainsKey(result)) {
                resultClauses.Add(result, new List<string>());
            }
            resultClauses[result].Add(clause);
        }
    }
}
