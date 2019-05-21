using System.Linq;

namespace UnifaceLibrary {
    
    internal class UnifaceCodeBlock
    {
        private static string[] _blockOrder = new[] 
        { 
            "INIT", "GENERAL", "MENU", "RETRIEVE", "STORE", "DELET", "CLEAR", "ASYNC", "ACCEPT", "SPRINT", "INTKEY", "QUIT",    // Readable code blocks
            "NEXTFLD", "PREVFLD", "STARTMOD", "DETAIL",                                                                         // Readable code blocks
            "FORMPIC", "TITLE", "TPLACTUAL", "LISTING", "PERF", "UCOMMENT", "WINPROP"                                           // Non-code blocks 
        };

        public string Name { get; set; }
        public string Code { get; set; }

        public UnifaceCodeBlock(string name, string code)
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
}