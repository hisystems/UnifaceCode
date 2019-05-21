using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnifaceLibrary
{
    internal class UnifaceObjectData
    {
        public Dictionary<string, object> Data { get; }

        /// <summary>
        /// Code is grouped for different aspects for example entity overrides on a form is for a different code block group.
        /// </summary>
        public IList<UnifaceObjectSourceCodeBlockGroup> SourceCodeBlockGroups { get; } = new List<UnifaceObjectSourceCodeBlockGroup>();

        public UnifaceObjectData(Dictionary<string, object> data, params UnifaceObjectSourceCodeBlockGroup[] sourceCodeGroups)
        {
            Data = data;
            SourceCodeBlockGroups = sourceCodeGroups.ToList();
        }
    }
}