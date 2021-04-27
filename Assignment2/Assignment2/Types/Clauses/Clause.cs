using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {

    /// <summary>
    /// The abstract interface of a clause object which defines some propositional truth in a knowledge base
    /// </summary>
    public abstract class Clause {

        /// <summary>
        /// The literal string which represents this clause. Includes polarity, to allow for differentiation between the same clause with different truth values.
        /// </summary>
        public string literal { get; protected set; }

        /// <summary>
        /// A boolean representing whether or not this clause is preceded by a negation symbol. This replaces the negation symbol in the identity, allowing opposing values to be compared.
        /// </summary>
        public bool polarity { get; protected set; }

        /// <summary>
        /// The identity of this clause. Does not include polarity, to allow for equality checking between same clauses with different truth values.
        /// </summary>
        public string identity { get; protected set; }

        /// <summary>
        /// Evaluate whether or not this clause is true for a given knowledge base
        /// </summary>
        /// <param name="KB">The knowledge base to check against</param>
        /// <returns>Returns the result of the evaluation, which may be null if a result could not be determined</returns>
        public abstract bool? Evaluate (KnowledgeBase KB);

        /// <summary>
        /// Determine if any new information can be inferred from this clause when it is true in a given knowledge base
        /// </summary>
        /// <param name="KB">The knowledge base to check against and update</param>
        /// <returns>Returns true if new information was added to the knowledge base</returns>
        public abstract bool Assert (KnowledgeBase KB);

        /// <summary>
        /// Return a clause which is the negation of this clause
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static Clause operator ~ (Clause clause) => GetClause(clause.GetLiteral(!clause.polarity));

        /// <summary>
        /// Construct a literal for this clause with a given polarity
        /// </summary>
        /// <param name="withPolarity">The polarity of the literal to be constructed, will use the clause's polarity if null</param>
        /// <returns>A literal string which represents this clause for the given polarity</returns>
        public abstract string GetLiteral (bool? withPolarity = null);

        /// <summary>
        /// A static list of references to all clause objects in the program, indexed by the literal string.
        /// </summary>
        private static Dictionary<string, Clause> clauses = new Dictionary<string, Clause>();

        /// <summary>
        /// Get all defined Clauses of a specific type T
        /// </summary>
        /// <typeparam name="T">The type of Clause to return</typeparam>
        /// <returns>A list of Clauses of type T</returns>
        public static List<T> GetAll<T>() where T : Clause => clauses.Values.Where(a => a.GetType() == typeof(T)).Select(a => a as T).ToList();

        /// <summary>
        /// Construct a new clause, and add it to the static list if it is unique.
        /// </summary>
        protected Clause (bool polarity, string identity) {
            this.polarity = polarity;
            this.identity = identity;
            this.literal = GetLiteral(polarity);

            if (!clauses.ContainsKey(literal)) {
                clauses.Add(literal, this);
            }
        }

        /// <summary>
        /// Get the static clause object that corresponds to a literal string
        /// </summary>
        /// <param name="literal">The string to parse and return as a clause object</param>
        /// <returns></returns>
        public static Clause GetClause (string literal) {

            // Check if a clause already exists for this literal
            if (!clauses.ContainsKey(literal)) {

                // Create a clause for this literal
                Clause clause = ParseString(literal);

                // Add the clause if it does not already exist (the literal may have been condensed)
                if (!clauses.ContainsKey(clause.literal)) {
                    clauses.Add(clause.literal, clause);
                }

                // Return the clause for this literal
                return clauses[clause.literal];
                
            }

            // Return the clause which already existed
            return clauses[literal];
        }

        /// <summary>
        /// Parse a string and convert it into a clause
        /// </summary>
        /// <param name="clause">The string form of a clause to be parsed</param>
        /// <returns>The generated clause object, either a rule or a symbol</returns>
        private static Clause ParseString (string clause) {

            // Decompose the string into a tree of substrings based on parenthesis
            Node<string> root = new Node<string>("", null);
            Node<string> current = root;

            // Build a tree of nodes based on nested parentheses
            foreach (char c in clause) {

                // If there was no parent node, end the loop
                if (current == null) {
                    break;
                }

                // If opening parentheses, add a placeholder child character, and create a child node
                if (c == '(') {
                    current.value += '■';
                    current.AddChild("");
                    current = current.children.Last();
                }

                // If closing parentheses, move back up to the parent of this node
                else if (c == ')') {
                    current = current.parent;
                }

                // Otherwise, continue to append characters to this node
                else {
                    current.value += c;
                }
            }

            // Return the clause generated by parsing this string tree
            return ParseStringNode(root);
        }

        /// <summary>
        /// Parse a String Node and convert it into a clause
        /// </summary>
        /// <param name="node">The root node of a string tree divided by parentheses</param>
        /// <returns>The generated clause object, either a rule or a symbol</returns>
        private static Clause ParseStringNode (Node<string> node) {

            string[] opSymbols = new string[5] { "<=>", "=>", "||", "&", "~" };
            string[] splitStrings = { };

            // Get the polarity and remove the negtion connective if applicable
            bool nodePolarity = node.NegateParentAndInvert(true);

            // Safeguard against clauses with brackets around the entire clause
            // Continue to flip the polarity if further negation symbols are found
            while (node.value == "■") {
                node = node.children[0];
                nodePolarity = node.NegateParentAndInvert(nodePolarity);
            }

            // Split the string of the root node using each connective, until one is found
            // This happens in the order of operations, to ensure logical order
            for (int i = 0; i < Enum.GetNames(typeof(Connective)).Length; i++) {

                splitStrings = node.value.Split(new String[] { opSymbols[i] }, 2, StringSplitOptions.RemoveEmptyEntries);

                // String was split, meaning the connective was found, and so this must be a rule
                if (splitStrings.Count() > 1) {

                    // Split the root into two sibling nodes, based on the connective found
                    Node<string>[] splitNodes = node.Split(opSymbols[i]);

                    // Return a new Rule using the polarity and logical components that were found
                    return new Rule(
                        nodePolarity,                   // polarity
                        ParseStringNode(splitNodes[0]), // antecedent
                        ParseStringNode(splitNodes[1]), // consequent
                        (Connective) i,                 // connective
                        node.Rebuild()                  // identity string
                        );
                }
            }

            // Never managed to split the string, must be a symbol
            if (splitStrings.Count() == 1) {

                // Check for the negation at the start to determine polarity
                // But do not include the negation symbol in the "clause" string reference
                return new Symbol(
                    node.value[0] != '~',                   // polarity
                    node.value.Replace("~", string.Empty)   // identity string
                    );
            }

            // No answer found - this should never happen
            return null;
        }
    }
}
