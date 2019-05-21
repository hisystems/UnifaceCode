using System;
using UnifaceLibrary;

namespace UnifaceGetObjectsChangedIds
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return BaseProgram<CommandLineOptions>.Run(args, options =>
            {
                var database = new UnifaceLibrary.UnifaceDatabase(options.DatabaseConnectionString);
                
                foreach (var unifaceObjectChange in database.GetAllObjectsChangedSince(options.SinceVersion))
                    Console.WriteLine($"{unifaceObjectChange.ChangeOperation.ToString()} {unifaceObjectChange.Object.Id}");

                return 0;
            });
        }
    }
}