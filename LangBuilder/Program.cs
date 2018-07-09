/**
 * -> This software uses a part from SteamKit2
 *    https://github.com/SteamRE/SteamKit
 *    This part has been edited for compiling with .NET Core v2.0
 *
 * This software uses MIT license.
 * Developed by CrazyHackGUT aka Kruzya.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace LangBuilder
{
    class Program
    {
        // Logging
        private static bool end = true;

        private static string BuildTimeLine()
        {
            var current = DateTime.Now;

            var date = $"{current.Day}/{current.Month}/{current.Year}";
            var time = $"{current.Hour}:{current.Minute}:{current.Second}";
            var dt = $"{date} - {time}";

            return dt;
        }

        public static void WriteLine(string data)
        {
            Write($"L {BuildTimeLine()}: {data}\n");
        }

        public static void Write(string data)
        {
            Console.Write(data);
            end = data.EndsWith('\n');
        }

        public static bool EOL()
        {
            return end;
        }

        // App
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(RunApp)
                .WithNotParsed<Options>(HandleParseError);
        }

        private static void RunApp(Options opts)
        {
            WriteLine("Starting...");
            new LangRead(opts).Run();
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            if (errors.Any(err => err.StopsProcessing))
            {
                return;
            }
        }
    }
}