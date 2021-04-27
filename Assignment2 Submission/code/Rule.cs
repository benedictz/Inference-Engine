using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {
    /// <summary>
    /// A sentence of prepositional logic, which can evaluate based on a connective, and may assert new clauses
    /// </summary>
    public class Rule : Clause {

        public Clause antecedent;
        public Clause consequent;
        public Connective connective;

        public Rule (bool polarity, Clause antecedent, Clause consequent, Connective connective, string identity) : base (polarity, identity) {
            this.antecedent = antecedent;
            this.consequent = consequent;
            this.connective = connective;
        }

        // Prepend the identity with a negation symbol if the polarity is negative, and surround it with parentheses
        public override string GetLiteral (bool? withPolarity = null) => ((withPolarity ?? polarity) ? "" : "~(") + identity + ((withPolarity ?? polarity) ? "" : ")");

        // Override the negation operator
        public static Rule operator ~ (Rule rule) => GetClause(rule.GetLiteral(!rule.polarity)) as Rule;

        // First return the polarity of this rule if it exists in the KB
        // Otherwise, evalute the rule using its connective
        public override bool? Evaluate (KnowledgeBase KB) {

            // Return the polarity of this rule if it exists in the KB already
            // Otherwise, evalute it using the logical connective
            // Apply the polarity using XNOR (negated XOR) with ternary logic, to preserve null values
            return KB.GetPolarity(this) ?? !(polarity ^ EvaluateRecursively(KB));
        }

        // Evaluate the antecedent and the consequent and determine the truth of some operaiton between them
        private bool? EvaluateRecursively (KnowledgeBase KB) {

            // Evaluate both the antecedent and the consequent
            bool? evalAnte = this.antecedent.Evaluate(KB);
            bool? evalCons = this.consequent.Evaluate(KB);

            // Compare the antecedent and the consequent using the correct connective
            switch (this.connective) {
                case (Connective.IMPLICATION):
                    return evalAnte.Implies(evalCons);

                case (Connective.BICONDITIONAL):
                    return evalAnte.Biconditional(evalCons);

                case (Connective.CONJUNCTION):
                    return (evalAnte & evalCons);

                case (Connective.DISJUNCTION):
                    return (evalAnte | evalCons);
            }

            // No result - should never happen
            return null;
        }

        // Determine if we can gain new information from this rule by evaluating parts based on the connective
        public override bool Assert (KnowledgeBase KB) {

            // Assert rule using the connective
            bool result = false;
            bool? evalAnte;
            bool? evalCons;

            switch (this.connective) {
                case (Connective.IMPLICATION):

                    if (antecedent.Evaluate(KB) == true) {
                        result |= polarity ?
                            KB.AddClause(consequent) :
                            KB.AddClause(~consequent);
                    }
                    else if (antecedent.Evaluate(KB) == false) {
                        if (!polarity) {
                            Console.WriteLine("Error [~IM~A]: Cannot assert the rule [" + literal + "] because it contradicts the Knowledge Base.");
                        }
                    }

                    if (consequent.Evaluate(KB) == false) {
                        result |= polarity ?
                            KB.AddClause(~antecedent) :
                            KB.AddClause(antecedent);
                    }
                    else if (consequent.Evaluate(KB) == true) {
                        if (!polarity) {
                            Console.WriteLine("Error [~IMC]: Cannot assert the rule [" + literal + "] because it contradicts the Knowledge Base.");
                        }
                    }

                    break;

                case (Connective.BICONDITIONAL):

                    evalAnte = antecedent.Evaluate(KB);
                    if (evalAnte != null) {
                        result |= (polarity == evalAnte) ?
                            KB.AddClause(consequent) :
                            KB.AddClause(~consequent);
                    }

                    evalCons = consequent.Evaluate(KB);
                    if (evalCons != null) {
                        result |= (polarity == evalCons) ?
                            KB.AddClause(antecedent) :
                            KB.AddClause(~antecedent);
                    }

                    break;

                case (Connective.CONJUNCTION):
                    result |= polarity ?
                        KB.AddClause(antecedent) :
                        KB.AddClause(~antecedent);

                    result |= polarity ?
                        KB.AddClause(consequent) :
                        KB.AddClause(~consequent);

                    break;

                case (Connective.DISJUNCTION):

                    evalAnte = antecedent.Evaluate(KB);
                    if (evalAnte == false) {
                        result |= polarity ?
                            KB.AddClause(consequent) :
                            KB.AddClause(~consequent);
                    }
                    else if (evalAnte == true) {
                        if (!polarity) {
                            Console.WriteLine("Error [~ORA]: Cannot assert the rule [" + literal + "] because it contradicts the Knowledge Base.");
                        }
                    }

                    evalCons = consequent.Evaluate(KB);
                    if (consequent.Evaluate(KB) == false) {
                        result |= polarity ?
                            KB.AddClause(consequent) :
                            KB.AddClause(~consequent);
                    }
                    else if (evalCons == true) {
                        if (!polarity) {
                            Console.WriteLine("Error [~ORC]: Cannot assert the rule [" + literal + "] because it contradicts the Knowledge Base.");
                        }
                    }

                    if (polarity) {
                        if ((evalCons & evalAnte) == false) {
                            Console.WriteLine("Error [OR~AC]: Cannot assert the rule [" + literal + "] because it contradicts the Knowledge Base.");
                        }
                    }

                    // TODO:
                    // Need to ensure that the assertation takes into account the polarity of this rule
                    if (antecedent.Evaluate(KB) == false) {
                        result |= KB.AddClause(consequent);
                    }
                    else if (consequent.Evaluate(KB) == false) {
                        result |= KB.AddClause(antecedent);
                    }

                    break;
            }
            
            return result;
        }
    }
}
