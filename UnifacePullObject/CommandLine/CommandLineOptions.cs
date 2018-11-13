using CommandLine;

namespace UnifacePullObject
{
    public class CommandLineOptions
    {
        [Option('d', "dbConnectionString", Required = true, HelpText = "Database connection string to Uniface repository database.")]
        public string DatabaseConnectionString { get; set; }

        [Option('o', "objectId", HelpText = "The unique object identifier of the format Type/Library/ObjectName e.g. 'Form/LPA/LPAC1000'. If omitted then stdin is used instead.")]
        public string ObjectId { get; set; }

        public bool UseStdIn => string.IsNullOrEmpty(ObjectId);

        [Option('c', "codeRootDir", Required = true, HelpText = "The root directory where the local Uniface source code resides.")]
        public string CodeRootDir { get; set; }
    }
}
