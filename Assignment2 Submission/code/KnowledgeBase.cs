using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {

    /// <summary>
    /// Stores a list of known clauses for a problem domain
    /// </summary>
    public class KnowledgeBase {

        /// <summary>
        /// A list of unique clauses in the KnowledgeBase, indexed by clause identity
        /// </summary>
        public Dictionary<string, Clause> clauses { get; }

        public KnowledgeBase (List<string> raws) {
            clauses = new Dictionary<string, Clause>();

            foreach (string raw in raws) {
                Clause c = Clause.GetClause(raw);
                if (!clauses.ContainsKey(c.identity)) {
                    clauses.Add(c.identity, c);
                }
                else {
                    if (clauses[c.identity].polarity != c.polarity) {
                        Console.WriteLine("Error [R~R]: Cannot construct with the clause [" + c.literal + "] because it contradicts the Knowledge Base.");
                    }
                }
            }
        }

        public KnowledgeBase (IEnumerable<Clause> iclauses) {
            clauses = new Dictionary<string, Clause>();

            foreach (Clause c in iclauses) {
                if (!clauses.ContainsKey(c.identity)) {
                    clauses.Add(c.identity, c);
                }
                else {
                    if (clauses[c.identity].polarity != c.polarity) {
                        Console.WriteLine("Error [C~C]: Cannot construct with the clause [" + c.literal + "] because it contradicts the Knowledge Base.");
                    }
                }
            }
        }

        /// <summary>
        /// Check the polarity of a given clause in the database
        /// </summary>
        /// <param name="clause">The clause to check</param>
        /// <returns>Returns Null if no matching clause is found, otherwise returns whether the found clause shares the same polarity</returns>
        public bool? GetPolarity (Clause clause) {
            if (clauses.ContainsKey(clause.identity)) {
                return clauses[clause.identity].polarity == clause.polarity;
            }
            return null;
        }

        /// <summary>
        /// Add a clause to the KnowledgeBase if it is unique
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public bool AddClause (Clause clause) {
            if (!clauses.ContainsKey(clause.identity)) {
                clauses.Add(clause.identity, clause);
                return true;
            }
            else {
                if (clauses[clause.identity].polarity != clause.polarity) {
                    Console.WriteLine("Error [A~A]: Cannot add the clause [" + clause.literal + "] because it contradicts the Knowledge Base.");
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a new KnowledgeBase with an additional clause
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public KnowledgeBase Extend (Clause clause) {
            return new KnowledgeBase(clauses.Values.Concat(new[] { clause }));
        }

        /// <summary>
        /// Determines if this KnowledgeBase entails some set of clauses
        /// Evaluates every clause in the KnowledgeBase with the given parameters 
        /// </summary>
        /// <param name="eKB">A given KnowledgeBase, which every clause is evaluated against</param>
        /// <returns>True if no clauses were disproven by the given KnowledgeBase</returns>
        public bool Entails (KnowledgeBase eKB) {

            // Evaluate every clause in this KB against the facts provided in eKB
            foreach (Clause clause in this.clauses.Values) {

                // This KB does not entail eKB if it proves any of these clauses false
                if (clause.Evaluate(eKB) == false) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if this KnowledgeBase entails some set of clauses
        /// Evaluates every clause in the KnowledgeBase with the given parameters 
        /// </summary>
        /// <param name="eClauses">A given clauses, which every clause is evaluated against</param>
        /// <returns>True if no clauses were disproven by the given set of clauses</returns>
        public bool Entails (IEnumerable<Clause> eClauses) {
            return Entails(new KnowledgeBase(eClauses));
        }

        /// <summary>
        /// Determines if this KnowledgeBase entails some set of clauses
        /// Evaluates every clause in the KnowledgeBase with the given parameters 
        /// </summary>
        /// <param name="eClause">A given clause, which every clause is evaluated against</param>
        /// <returns>True if no clauses were disproven by the given clause</returns>
        public bool Entails (Clause eClause) {
            return Entails(new[] { eClause });
        }

    }
}
