namespace DevTools.JsonMerge.Cli
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class Program
    {
        private static void CrashAndBurn(ExitCode code, string crashMessage, params object[] args)
        {
            // Preserve the foreground color
            ConsoleColor c = Console.ForegroundColor;

            // Write out our error message in bright red text
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + crashMessage, args);

            // Restore the foreground color
            Console.ForegroundColor = c;

            // Die!
            Environment.Exit((int)code);
        }

        private static JsonMergeOptions GetOptions(IReadOnlyList<string> args)
        {
            var options = new JsonMergeOptions();
            for (int i = 0; i < args.Count; i++)
            {
                string key = args[i];
                if ((key == "-i") || (key == "-input"))
                {
                    if (args.Count == i + 1)
                    {
                        CrashAndBurn(ExitCode.InputArgumentMissing, "Value for option 'input' is missing");
                    }
                    options.Inputs.Add(args[i + 1]);
                    i++;
                    continue;
                }

                if ((key == "-file") || (key == "-outputFile"))
                {
                    if (args.Count == i + 1)
                    {
                        CrashAndBurn(ExitCode.InvalidOutputArgument, "Value for option 'outputFile' is missing");
                        Console.WriteLine("ERROR: Value for option 'outputFile' is missing");
                        Environment.Exit(-2);
                    }
                    options.OutputFile = args[i + 1];
                    i++;
                    continue;
                }
            }

            return options;
        }

        private static void Main(string[] args)
        {
            if ((args.Length == 0) || ((args.Length == 1) && ((args[0] == "-help") || (args[0] == "-?"))))
            {
                PrintHelp();
            }
            JsonMergeOptions options = GetOptions(args);
            CheckOptions(options);

            var jsonMerger = new JsonMerge(options);
            bool success = jsonMerger.Convert();
            string message = success
                ? "Json files merged successfully"
                : "Chould not merge json files";

            ConsoleColor backupColor = Console.ForegroundColor;
            Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = backupColor;
        }

        private static void CheckOptions(JsonMergeOptions options)
        {
            if (options.Inputs.Count > 0)
            {
                foreach (string input in options.Inputs)
                {
                    string path = input;
                    if (!Path.IsPathRooted(input))
                    {
                        path = Path.Combine(Environment.CurrentDirectory, input);
                    }

                    if (Directory.Exists(path))
                    {
                        string[] directoryFiles = Directory.GetFiles(path, "*.json");
                        options.InputFiles.AddRange(directoryFiles);
                    }
                    else if (File.Exists(path))
                    {
                        options.InputFiles.Add(path);
                    }
                    else
                    {
                        CrashAndBurn(ExitCode.InvalidInputPath, "input path '{0}' doesn't relate to a file or a directory", path);
                    }
                }
            }
            else
            {
                string[] directoryFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.json");
                options.InputFiles.AddRange(directoryFiles);
            }

            if (string.IsNullOrEmpty(options.OutputFile))
            {
                CrashAndBurn(ExitCode.InvalidOutputArgument, "output file must be defined");
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(
                @"JsonMerge.Cli 2016
A command line interface for combining json files.
USAGE:
  -input or -i              - path to directory with *.json files or to separate file 
                              HINT: there can be several such options specifed at once
  -outputFile or -file      - path to output file (instead of outputDir)


EXAMPLES:
JsonMerge.Cli.exe -i .\Server -file c:\src\combined.json
Processes all *.json in folder 'Server' (relative to current dir) and creates a single json-file in 'c:\src\combined.json'
");
            Environment.Exit(0);
        }

        private enum ExitCode
        {
            InputArgumentMissing = -1,
            InvalidOutputArgument = -2,
            InvalidInputPath = -3
        }
    }
}
