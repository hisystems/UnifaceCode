using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnifaceLibrary {

    public class UnifaceSourceCodeParser {

        internal static IEnumerable<UnifaceCodeBlock> GetMetaDataNodes(StringBuilder sourceCode, string codeBlockGroupName = "")
        {
            sourceCode.Replace("<unimeta>", string.Empty).Replace("</unimeta>", String.Empty);
            var nodeSections = new Regex(@"<(\w*)>(?<code>.*)<\/\1>");
            var codeBlockPrefix = String.IsNullOrEmpty(codeBlockGroupName) ? "" : codeBlockGroupName + ".";

            foreach (var match in nodeSections.Matches(sourceCode.ToString()).Cast<Match>())
            {
                yield return new UnifaceCodeBlock(codeBlockPrefix + match.Groups[1].Value, match.Groups["code"].Value.UnescapeXml());
            }
        }

        /// <summary>
        /// Assumes that the reader records are ordered correctly.
        /// </summary>
        internal static void LoadOverflowSegments(IDataReader reader, StringBuilder sourceCode) {
            
            while (reader.Read()){
                // Remove the `Next Segment ID` 4 bytes from the end of the last added segment (not applicable for the last line).
                if (sourceCode.Length > 0)
                    sourceCode.Remove(sourceCode.Length - 4, 4);

                sourceCode.Append(reader["data"].ToString());
            }
        }
    }
}