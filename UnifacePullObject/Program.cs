using System;
using System.IO;
using UnifaceLibrary;

namespace UnifacePullObject
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return BaseProgram<CommandLineOptions>.Run(args, options =>
            {
                var database = new UnifaceLibrary.UnifaceDatabase(options.DatabaseConnectionString);
                var codeRootDir = new DirectoryInfo(options.CodeRootDir);

                Action<string> pullObject = (string objectIdString) => {
                    var objectId = UnifaceObjectId.Parse(objectIdString);
                    database.PullObject(objectId, codeRootDir);

                    Console.WriteLine($"Pulled {objectId}");
                };

                if (options.UseStdIn)
                {
                    string objectIdLine;

                    while (!string.IsNullOrEmpty(objectIdLine = Console.ReadLine()))
                        pullObject(objectIdLine);
                }
                else
                {
                    pullObject(options.ObjectId);
                }

                return 0;
            });
        }
    }
}