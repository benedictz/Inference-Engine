using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {
    /// <summary>
    /// The smallest unit of propositional logic which can only evaluate itself, and can assert no new information
    /// </summary>
    public class Symbol : Clause {

        public Symbol (bool polarity, string identity) : base (polarity, identity) { }

        // Prepend the identity with a negation symbol if the polarity is negative
        public override string GetLiteral (bool? withPolarity = null) => ((withPolarity??polarity) ? "" : "~") + identity;

        // Ask the KB if this symbol exists, and if so whether or not it shares the same polarity
        public override bool? Evaluate (KnowledgeBase KB) => KB.GetPolarity(this);

        // Cannot assert new information as this is a singular unit of propositional logic.
        public override bool Assert (KnowledgeBase KB) => false;

        // Override the negation operator
        public static Symbol operator ~ (Symbol symbol) => GetClause(symbol.GetLiteral(!symbol.polarity)) as Symbol;
    }
}
