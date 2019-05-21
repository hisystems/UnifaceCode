using System.Text;

namespace UnifaceLibrary
{
    internal class UnifaceObjectSourceCodeBlockGroup
    {
        /// <summary>
        /// The main group will have no name.
        /// Entity group overrides or entity field overrides will have different group prefixes.
        /// </summary>
        public string Name { get; }

        public StringBuilder SourceCode { get; }

        public UnifaceObjectSourceCodeBlockGroup(string name, StringBuilder sourceCode)
        {
            Name = name;
            SourceCode = sourceCode;
        }
    }
}