using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Contains all of the custom types and extension methods for this program
namespace Assignment2 {
    
    /// <summary>
    /// A binary logical operator that defines the relationship between two clauses 
    /// </summary>
    public enum Connective {
        BICONDITIONAL = 0,
        IMPLICATION = 1,
        DISJUNCTION = 2,
        CONJUNCTION = 3
    }

    /// <summary>
    /// A simple node of generic type T, with 0..1 parent nodes and 0..* child nodes, forming a tree structure
    /// </summary>
    /// <typeparam name="T">The type of variable stored in this tree</typeparam>
    public class Node<T> {

        /// <summary>
        /// The value of generic type T, contained within this node
        /// </summary>
        public T value;

        /// <summary>
        /// The parent node, of which this node is a child (read-only)
        /// </summary>
        public Node<T> parent { get; }

        /// <summary>
        /// The list of child nodes, to whom this node is a parent (read-only)
        /// </summary>
        public List<Node<T>> children { get; }

        public Node (T value, Node<T> parent = null) {
            this.value = value;
            this.parent = parent;
            this.children = new List<Node<T>>();
        }

        /// <summary>
        /// Appaend a child node to this node
        /// </summary>
        /// <param name="value">The value to be contained in the new child node</param>
        public void AddChild (T value) {
            children.Add(new Node<T>(value, this));
        }
    }

    /// <summary>
    /// Provides extension methods for String Nodes to be split into sibling nodes or rebuilt into strings
    /// </summary>
    public static class SplittableExtension {

        const char childSymbol = '■';

        /// <summary>
        /// Split one node into an array of nodes, based on some delimiter of the string value
        /// </summary>
        /// <param name="delimiter">Used to split the value of the node</param>
        /// <param name="childSymbol">The symbol in the node value which represents a child node</param>
        /// <returns>Returns a list of nodes, parented to the original parent, with the respective children attached</returns>
        public static Node<string>[] Split (this Node<string> node, string delimiter, int maxSplit = 2) {

            // Split the value using a delimiter
            string[] values = node.value.Split(new string[] { delimiter }, maxSplit, StringSplitOptions.RemoveEmptyEntries);
            int child = 0;
            List<Node<string>> result = new List<Node<string>>();

            // For each string that was split, create a new new node
            foreach (string s in values) {

                // Create a new node for each split in the string value
                // Each new node will still have the same parent
                result.Add(new Node<string>(s, node.parent));

                // In each new node, copy over the relevant children from the source node,
                // based on the number of child markers found
                foreach (char c in s) {
                    if (c == childSymbol) {
                        result.Last().children.Add(node.children[child++]);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Recursively rebuilds the original string with parentheses around each child
        /// </summary>
        /// <returns>The string containing all children before being divided into a tree</returns>
        public static string Rebuild (this Node<string> node) {

            // Split the root string for every instance of the child node delimiter
            string[] array = node.value.Split(new char[] { childSymbol });

            // Start with the first piece of the string, before the first child
            string result = array[0];

            // For every child and piece of the split string
            for (int i = 0; (i < array.Count() - 1) && (i < node.children.Count()); i++) {

                // Put parentheses around that child, rebuild that child, and add the next piece of the root string
                result += "(";
                result += node.children[i].Rebuild();
                result += ")";
                result += array[i + 1];
            }

            return result;
        }

        /// <summary>
        /// Trim the negation symbol from the node's value if it is just a negation of a single child
        /// </summary>
        /// <param name="node">The node to check for negation trimming</param>
        /// <param name="toInvert">The current polarity, to be inverted if a negation symbol is found</param>
        /// <returns>Returns the resulting polarity, either the toInvert value if no negation symbol was found, or an inverted value if one was found.</returns>
        public static bool NegateParentAndInvert (this Node<string> node, bool toInvert) {

            // TODO: Test this function with some stupid double negatives
            if (node.value == "~■") {
                node.value = "■";
                toInvert = !toInvert;
            }

            return toInvert;
        }
    }



    public static class BooleanNullLogicExtensions {

        /// <summary>
        /// Evaluate the result of "A → B" using a Kleene truth table for ternary logic
        /// </summary>
        public static bool? Implies(this bool? a, bool? b) {
            return (!a | b);
        }

        /// <summary>
        /// Evaluate the result of "A ⇔ B" by combining Kleene truth tables for implication with a conjunction
        /// </summary>
        public static bool? Biconditional (this bool? a, bool? b) {
            return (a.Implies(b) & b.Implies(a));
        }
    }

    public static class DictionaryExtensions {

        /// <summary>
        /// Returns the value for a given key, adding a new key-value pair to the dictionary if one does not exist
        /// </summary>
        /// <param name="key">The key of a key value pair to search for</param>
        /// <returns>A value type from the dictionary, located at the key</returns>
        public static T GetAdd<K,T> (this Dictionary<K,T> dictionary, K key) where T : new() {
            if (dictionary.ContainsKey(key)) {
                return dictionary[key];
            }
            else {
                dictionary.Add(key, new T());
                return dictionary[key];
            }  
        }

        /// <summary>
        /// Returns the value for a given key, or a new value if the key-value pair does not exist
        /// </summary>
        /// <param name="key">The key of a key value pair to search for</param>
        /// <returns>A value type from the dictionary, either located at the key, or a new value if the key was not found</returns>
        public static T GetNew<K, T> (this Dictionary<K, T> dictionary, K key) where T : new() {
            if (dictionary.ContainsKey(key)) {
                return dictionary[key];
            }
            else {
                return new T();
            }
        }
    }
}
