using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tahvohck_Mods;

namespace PBModBuildHelper
{
    public class Options
    {
        [Value(0, Required = true, HelpText = "The path of the assembly to analyze.", MetaName ="Path")]
        public string Path { get; set; }

        [Option('v', "verbose")]
        public static bool Verbose { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);
            result
                .WithParsed(ParseSucces)
                .WithNotParsed(ParseFailure);

            Environment.Exit(0);
        }

        private static void ParseFailure(IEnumerable<Error> obj)
        {
            foreach(Error e in obj) {
                if (e is HelpRequestedError) break;
                Error($"{e}");
            }
            Environment.Exit(1);
        }

        static void ParseSucces(Options opt)
        {
            BindingFlags filter =
                BindingFlags.DeclaredOnly |
                BindingFlags.Static |
                BindingFlags.Public;

            FileInfo file = new FileInfo(opt.Path);
            if (file.Exists) {
                var assem = Assembly.LoadFrom(file.FullName);
                var types = assem.GetTypes().ToList();
                var allMethods = types.SelectMany(t => t.GetMethods(filter)).ToList();

                if (Options.Verbose) {
                    Console.WriteLine($"Loaded assembly. Found {types.Count} types and" +
                        $" {allMethods.Count} Methods.");
                }

                MethodInfo oneMethod = allMethods.Find(
                    m =>
                    m.CustomAttributes.ToList().FindAll(
                        a =>
                        a.AttributeType == typeof(EntryMethodAttribute))
                    .ToList().Count > 0
                    );

                if (oneMethod is null) {
                    Error("ERROR FINDING ENTRY");
                } else {
                    Console.WriteLine($"{oneMethod?.DeclaringType.FullName}.{oneMethod?.Name}");
                }
            } else {
                Error("ERROR PATH DOES NOT EXIST");
            }
        }

        static void Error(object obj)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj);
            Console.ForegroundColor = tmp;
        }
    }
}
