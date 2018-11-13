using System;
using CommandLine;

namespace UnifaceLibrary
{
    public static class BaseProgram<T>
        where T : new()
    {
        public static int Run(string[] args, Func<T, int> run)
        {
            var parser = new Parser(with =>
            {
                with.CaseSensitive = false;     // Case-insensitive argument names.
                with.HelpWriter = Console.Error;
            });

            var result = parser.ParseArguments<T>(args);

            if (result is NotParsed<T>)
                return 1;       // Failed, errors are automatically written to Console.Error

            var options = ((Parsed<T>)result).Value;

            // try
            {
                return run(options);
            }
            // catch (Exception ex)
            // {
            //     Console.Error.WriteLine(ex.Message);
            //     return ex.HResult;
            // }
        }
    }
}
