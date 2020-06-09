using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PBModBuildHelper
{
    public class Options
    {
        public static string Path = "C:/";
        public static bool Verbose = false;
        public readonly static Type entryTag = typeof(LoaderOptimizationAttribute);
    }


    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("-v") || args.Contains("--verbose")) {
                Options.Verbose = true;
            }

            if (args.Contains("-V")) {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                Console.WriteLine(v);
                Environment.Exit(0);
            }

            foreach (string s in args) {
                if (File.Exists(s)) {
                    Options.Path = s;
                    break;
                }
            }
            ParseSucces();
            Environment.Exit(0);
        }

        static void ParseSucces()
        {
            BindingFlags filter =
                BindingFlags.DeclaredOnly |
                BindingFlags.Static |
                BindingFlags.Public;

            FileInfo file = new FileInfo(Options.Path);
            if (file.Exists) {
                var assem = Assembly.LoadFrom(file.FullName);
                var types = assem.GetTypes().ToList();
                var allMethods = types.SelectMany(t => t.GetMethods(filter)).ToList();

                if (Options.Verbose) {
                    Console.WriteLine($"Loaded assembly. Found {types.Count} types and" +
                        $" {allMethods.Count} Methods.");
                }

                if (Options.Verbose) {
                    foreach (var method in allMethods) {
                        Console.WriteLine($"-------------------------" +
                            $"\n-> {method?.DeclaringType.FullName ?? "none"}." +
                            $"{method?.Name ?? "none"}");
                        foreach (Attribute a in method.GetCustomAttributes(false)) {
                            Console.WriteLine($"\t{a}");
                        }
                    }
                    Console.WriteLine();
                }

                MethodInfo oneMethod = allMethods.Find(
                    m =>
                    m.GetCustomAttributes(false).ToList().FindAll(
                        a =>
                        a.GetType().FullName == Options.entryTag.FullName)
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
