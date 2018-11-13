using CommandLine;

namespace UnifaceGetAllObjectIds
{
    public class CommandLineOptions
    {
        [Option('d', "dbConnectionString", Required = true, HelpText = "Database connection string to Uniface repository database.")]
        public string DatabaseConnectionString { get; set; }
    }
}