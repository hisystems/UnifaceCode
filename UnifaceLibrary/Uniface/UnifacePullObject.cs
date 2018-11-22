using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace UnifaceLibrary
{
    internal static class UnifacePullObject
    {
        private class ObjectData
        {
            public Dictionary<string, object> Data { get; }
            public StringBuilder SourceCode { get; }

            public ObjectData(Dictionary<string, object> data, StringBuilder sourceCode)
            {
                Data = data;
                SourceCode = sourceCode;
            }
        }

        private class CodeBlock
        {
            private static string[] _blockOrder = new[] 
            { 
                "INIT", "GENERAL", "MENU", "RETRIEVE", "STORE", "DELET", "CLEAR", "ASYNC", "ACCEPT", "SPRINT", "INTKEY", "QUIT",    // Readable code blocks
                "FORMPIC", "TITLE", "TPLACTUAL", "LISTING", "PERF", "UCOMMENT", "WINPROP"                                           // Non-code blocks 
            };

            public string Name { get; set; }
            public string Code { get; set; }

            public CodeBlock(string name, string code)
            {
                Name = name;
                Code = code;
            }

            public int Order 
            {
                get
                {
                    return _blockOrder
                        .Select((blockName, index) => new { blockName, BlockIndex = index })
                        .Where(b => b.blockName == Name)
                        .FirstOrDefault()?.BlockIndex ?? _blockOrder.Length + 1;
                }
            }
        }

        public static void PullObject(SqlConnection connection, UnifaceObjectId objectId, DirectoryInfo codeRootDirectory)
        {
            var objectData = ExtractSourceCode(connection, objectId);
            var objectDirectory = codeRootDirectory.CombinePath(objectId.Type.Name).CombinePath(objectId.LibraryName).CombinePath(objectId.ObjectName);
            objectDirectory.Create();

            using (var codeFile = new AtomicFileWrite(objectDirectory.CombineFile($"{objectId.ObjectName}.uni")))
            using (var stream = new StreamWriter(codeFile.OpenWrite()))
            {
                foreach (var codeBlock in GetMetaDataNodes(objectData.SourceCode).OrderBy(s => s.Order))
                {
                    stream.WriteLine($"\r\n;~~~~~~~~~~~~~~~~~~~~{codeBlock.Name}~~~~~~~~~~~~~~~~~~~~");       // Special header lines that indicate the block lines.
                    stream.Write(codeBlock.Code.TransformUnifaceLineEndings());
                }

                codeFile.Commit();
            }

            using (var dataFile = new AtomicFileWrite(objectDirectory.CombineFile($"{objectId.ObjectName}.json")))
            using (var stream = new StreamWriter(dataFile.OpenWrite()))
            {
                var exportData = objectData.Data
                    .Where(a => a.Key != objectId.Type.TableSource.IdField)
                    .Where(a => a.Key != objectId.Type.TableSource.CodeField)
                    .OrderBy(a => a.Key)
                    .ToDictionary(a => a.Key, a => TransformDataValue(a.Value));

                stream.Write(JsonConvert.SerializeObject(exportData, Formatting.Indented));

                dataFile.Commit();
            }
        }

        private static object TransformDataValue(object value)
        {
            if (value == DBNull.Value)
                return null;
            else if (value is string)
                return value.ToString().Trim();
            else
                return value;
        }

        private static IEnumerable<CodeBlock> GetMetaDataNodes(StringBuilder sourceCode)
        {
            sourceCode.Replace("<unimeta>", String.Empty).Replace("</unimeta>", String.Empty);
            var nodeSections = new Regex(@"<(\w*)>(?<code>.*)<\/\1>");

            foreach (var match in nodeSections.Matches(sourceCode.ToString()).Cast<Match>())
            {
                yield return new CodeBlock(match.Groups[1].Value, match.Groups["code"].Value.UnescapeXml());
            }
        }

        private static ObjectData ExtractSourceCode(SqlConnection connection, UnifaceObjectId objectId)
        {
            var sourceCode = new StringBuilder();
            Dictionary<string, object> objectData;

            var tableSource = objectId.Type.TableSource;

            var libraryCondition = objectId.LibraryNameIsGlobal ?
                $"{tableSource.LibraryField} IS NULL" : $"{tableSource.LibraryField} = '{objectId.LibraryName}'";
            
            var command = new SqlCommand(
                $"SELECT * FROM {tableSource.PrimaryTable} "+
                $"WHERE {tableSource.IdField} = '{objectId.ObjectName}' AND {libraryCondition} AND {tableSource.TypeFilter}", connection);

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                    throw new InvalidOperationException($"Could not find {objectId} in database. Check that the object identifier is correct. SQL: '{command.CommandText}'");

                objectData = reader.ToDictionary();
                sourceCode.Append(reader[tableSource.CodeField].ToString());
            }

            var dumpSegmentsCommand = new SqlCommand($"SELECT data FROM {tableSource.OverflowTable} WHERE {tableSource.IdField} = '{objectId.ObjectName}' ORDER BY segm", connection);

            using (var reader = dumpSegmentsCommand.ExecuteReader())
            {
                while (reader.Read()){
                    // Remove the `Next Segment ID` 4 bytes from the end of the last added segment (not applicable for the last line).
                    if (sourceCode.Length > 0)
                        sourceCode.Remove(sourceCode.Length - 4, 4);

                    sourceCode.Append(reader["data"].ToString());
                }
            }

            return new ObjectData(objectData, sourceCode);
        }
    }
}