using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2 {
    public abstract class SearchMethod {

        public List<string> path = new List<string>();

        /// <summary>
        /// Search the knowledge base using the query, and return the output
        /// </summary>
        /// <returns>String output for the console</returns>
        public abstract string Search (KnowledgeBase KB, Clause query);
    }
}
