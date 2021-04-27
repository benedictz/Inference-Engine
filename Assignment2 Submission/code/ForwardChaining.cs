using System;
using System.Collections.Generic;

namespace Assignment2 {

    public class ForwardChaining : SearchMethod {

        /// <summary>
        /// Determines whether some query "q" is entailed by some knowledge base, assumed to be in horn form
        /// </summary>
        public override string Search (KnowledgeBase KB, Clause query) {

            // Convert the horn form knowledge base into a relational database
            HornDatabase hdb = new HornDatabase(KB);

            // Define symbols for the algorithm 
            string q = query.identity;
            string p = "";

            // Populate the agenda with the symbols in the knowledge base
            Queue<string> agenda = new Queue<string>(hdb.facts);

            // Check if the query is already a known fact
            if (hdb.facts.Contains(q)) {
                path.Add(q);
                return $"YES: {string.Join(", ", path)}";
            }

            // While agenda is not empty do
            while (agenda.Count > 0) {

                // "p" ← Pop(agenda)
                p = agenda.Dequeue();

                // Unless inferred["p"]
                if (!path.Contains(p)) {

                    // Inferred["p"] ← true
                    path.Add(p);

                    // For each horn clause "c" in whose premise "p" appears do
                    foreach (string c in hdb.symbolClauses.GetNew(p)) {

                        // Decrement count["c"]
                        // If count["c"] is 0 then do
                        if (--hdb.unresolvedCount[c] == 0) {

                            // If result["c"] is "q" then return true
                            if (hdb.clauseResult[c] == q) {
                                path.Add(q);
                                return $"YES: {string.Join(", ", path)}";
                            }

                            // Push(result["c"], agenda)
                            agenda.Enqueue(hdb.clauseResult[c]);
                        }
                    }
                }
            }

            // Return false
            return "NO";

        }
    }
}
