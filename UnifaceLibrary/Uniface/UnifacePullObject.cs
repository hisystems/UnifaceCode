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
        public static void PullObject(SqlConnection connection, UnifaceObjectId objectId, DirectoryInfo codeRootDirectory)
        {
            var objectData = objectId.Type.SourceCode.Extract(connection, objectId);
            var objectDirectory = codeRootDirectory.CombinePath(objectId.Type.Name).CombinePath(objectId.LibraryName).CombinePath(objectId.ObjectName);
            objectDirectory.Create();

            using (var codeFile = new AtomicFileWrite(objectDirectory.CombineFile($"{objectId.ObjectName}.uni")))
            using (var stream = new StreamWriter(codeFile.OpenWrite()))
            {
                foreach (var codeBlockGroup in objectData.SourceCodeBlockGroups) {
                    foreach (var codeBlock in UnifaceSourceCodeParser.GetMetaDataNodes(codeBlockGroup.SourceCode).OrderBy(s => s.Order))
                    {
                        stream.WriteLine($"\r\n;~~~~~~~~~~~~~~~~~~~~{codeBlockGroup.Name}{codeBlock.Name}~~~~~~~~~~~~~~~~~~~~");       // Special header lines that indicate the block lines.
                        stream.Write(codeBlock.Code.TransformUnifaceLineEndings());
                    }
                }

                codeFile.Commit();
            }

            using (var dataFile = new AtomicFileWrite(objectDirectory.CombineFile($"{objectId.ObjectName}.json")))
            using (var stream = new StreamWriter(dataFile.OpenWrite()))
            {
                var exportData = objectData.Data
                    .Where(a => a.Key != objectId.Type.SourceCode.IdField)
                    .Where(a => a.Key != objectId.Type.SourceCode.CodeField)
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
    }
}