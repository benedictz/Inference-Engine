using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {
    class ResolutionForwardChaining : SearchMethod {

        public override string Search (KnowledgeBase KB, Clause query) {
            bool? queryResult = query.Evaluate(KB);
            if (queryResult.HasValue) {
                path.Add(query.literal);
                return ((bool) queryResult ? "TRUE: " + string.Join(", ", path) : "FALSE");
            }

            foreach (Clause clause in KB.clauses.Values) {
                if (!path.Contains(clause.literal)) {
                    if (clause.Assert(KB)) {

                        path.Add(clause.literal);
                        return Search(KB, query);
                    }
                }
            }

            return "UNKNOWN";
        }
    }
}
