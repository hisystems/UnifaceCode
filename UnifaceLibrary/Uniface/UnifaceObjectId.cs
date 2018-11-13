using System;
using System.IO;
using System.Linq;

namespace UnifaceLibrary
{
    public struct UnifaceObjectId
    {
        private const string _libraryNameGlobal = "(GLOBAL)";

        public UnifaceObjectType Type { get; private set; }
        public bool LibraryNameIsGlobal => LibraryName == _libraryNameGlobal;
        public string LibraryName { get; private set; }
        public string ObjectName { get; private set; }

        /// <param name="libraryName">If null then changes to library '(GLOBAL)'</param>
        public UnifaceObjectId(UnifaceObjectType type, string libraryName, string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName was not specified", nameof(objectName));

            Type = type ?? throw new ArgumentNullException(nameof(type));
            LibraryName = String.IsNullOrEmpty(libraryName) ? _libraryNameGlobal : libraryName?.Trim();
            ObjectName = objectName.Trim();
        }

        /// <summary>
        /// Path uniquely identifies a Uniface objects.
        /// Typically of the format Type\Library\ObjectName. 
        /// This will point to a folder that contains the actual .uni files.
        /// </summary>
        public static UnifaceObjectId Parse(string path)
        {
            var foldersHierarchy = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, count: 3);

            if (foldersHierarchy.Length == 3 && foldersHierarchy.All(_ => !String.IsNullOrEmpty(_)))
            {
                var type = UnifaceObjectType.Get(foldersHierarchy[0]);

                return new UnifaceObjectId(type, libraryName: foldersHierarchy[1], objectName: foldersHierarchy[2]);
            }

            throw new FormatException($@"'{path}' expected format is 'Type/Library/ObjectName'");
        }

        public override string ToString()
        {
            return $"{Type.Name}/{LibraryName}/{ObjectName}";
        }
    }
}