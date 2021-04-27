using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {
    public class BackwardChaining : SearchMethod {

        public override string Search(KnowledgeBase KB, Clause query) {

            // Convert the horn form knowledge base into a relational database
            HornDatabase hdb = new HornDatabase(KB);

            // Define symbols for the algorithm
            string q = query.identity;
            string p = "";
            string d = "";

            // Populate the agenda with the query and define the open and closed lists
            Queue<string> agenda = new Queue<string>(new string[] { q });
            List<string> closed = new List<string>();
            Queue<string> open = new Queue<string>();

            // Check if the query is already a known fact
            if (hdb.facts.Contains(q)) {
                path.Add(q);
                return $"YES: {string.Join(", ", path)}";
            }

            // While agenda is not empty do
            while (agenda.Count > 0) {

                // "p" ← Pop(agenda)
                p = agenda.Dequeue();
                closed.Insert(0, p);

                // For each horn clause "c" which proves "p" do
                foreach (string c in hdb.resultClauses.GetNew(p)) {

                    // For each symbol "s" which appears in the premise of "c"
                    foreach (string s in hdb.clauseSymbols.GetNew(c)) {

                        // If KB entails "s"
                        if (hdb.facts.Contains(s)) {

                            // Inferred["s"] ← true
                            if (!path.Contains(s)) { path.Add(s); }

                            // Push("c", open)
                            open.Enqueue(c);

                            // While open is not empty do
                            while (open.Count > 0) {

                                // "d" ← Pop(open)
                                d = open.Dequeue();

                                // Decrement count["d"]
                                // If count["d"] is 0 then do
                                if (--hdb.unresolvedCount[d] == 0) {

                                    // Inferred["d"] ← true
                                    if (!path.Contains(hdb.clauseResult[d])) { path.Add(hdb.clauseResult[d]); }

                                    // If result["d"] is "q" then return true
                                    if (hdb.clauseResult[d] == q) {
                                        return $"YES: {string.Join(", ", path)}";
                                    }

                                    // For each horn clause "e" in whose premise the conclusion of "d" appears do
                                    foreach (string e in hdb.symbolClauses.GetNew(hdb.clauseResult[d])) {

                                        // Push("e", open)
                                        open.Enqueue(e);
                                    }
                                }
                            }
                        }

                        // Unless closed["s"] or agenda["s"]
                        else if (!closed.Contains(s) && !agenda.Contains(s)) {

                            // Push("s", agenda)
                            agenda.Enqueue(s);
                        }
                    }
                }
            }
            return "NO";
        }
    }
}
