using System;
using UnifaceLibrary;

namespace UnifaceGetAllObjectIds
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return BaseProgram<CommandLineOptions>.Run(args, options =>
            {
                var database = new UnifaceLibrary.UnifaceDatabase(options.DatabaseConnectionString);
                
                foreach (var unifaceObject in database.GetAllObjects())
                    Console.WriteLine(unifaceObject.Id);

                return 0;
            });
        }
    }
}
