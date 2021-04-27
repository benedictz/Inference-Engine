using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Assignment2 {

    /// <summary>
    /// Opens a text file and reads the data into a knowledge base
    /// </summary>
    public static class FileHandler {

        // The file path and array of strings from the file
        private static string       filePathway;
        private static string[]     textArray;

        // Public properties for the data read from file
        public static string Query { get; private set; }
        public static List<string> Clauses { get; private set; }

        /// <summary>
        /// Checks if the file exists and if so, store the path for later use
        /// </summary>
        /// <param name="passedInPath">The relative path for the file</param>
        /// <returns>Whether the file exists</returns>
        public static bool FilePathwayExists (string passedInPath) {
            bool exists = false;
            if (File.Exists(passedInPath)) {
                filePathway = passedInPath;
                exists = true;
            }
            return exists;
        }

        /// <summary>
        /// Parse the text file to find the list of clauses and query
        /// </summary>
        public static void ParseFile() {

            // Remove all whitespaces
            // Splits into array using ";" as splitter
            // Remove any null entries in array
            // Store the clauses
            textArray = File.ReadAllLines(filePathway);
            textArray[1] = textArray[1].Replace(" ", string.Empty);
            Clauses = Regex.Split(textArray[1], ";").Where(s => !string.IsNullOrEmpty(s)).ToList();

            // Remove all whitespaces
            // Store the query
            Query = textArray[3].Replace(" ", string.Empty);
        }
    }
}
