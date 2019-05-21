using CommandLine;

namespace UnifaceGetObjectsChangedIds
{
    public class CommandLineOptions
    {
        [Option('d', "dbConnectionString", Required = true, HelpText = "Database connection string to Uniface repository database.")]
        public string DatabaseConnectionString { get; set; }

        [Option('v', "sinceVersion", Required = true, HelpText = "Only objects that have changed since the specified SQL SERVER change tracking version.")]
        public int SinceVersion { get; set; }
    }
}