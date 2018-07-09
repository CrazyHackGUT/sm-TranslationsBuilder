using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.IO;
using CSharpx;
using SteamKit2;

namespace LangBuilder
{
    public class LangBuild
    {
        private readonly Options _opts;
        private Dictionary<string, List<string>> _formatRules;
        private Dictionary<string, Dictionary<string, string>> _languages;

        public LangBuild(Options opts, Dictionary<string, List<string>> rules,
            Dictionary<string, Dictionary<string, string>> lang)
        {
            _opts = opts;
            _formatRules = rules;
            _languages = lang;
        }

        public void Run()
        {
            // DO BUILD LOGIC!
            Check();
            Program.WriteLine("[Builder] Running...");

            BuildFormatRules();
            BuildLanguages();
        }

        private void Check()
        {
            if (!Directory.Exists(_opts.Output))
                Directory.CreateDirectory(_opts.Output);
        }

        private void BuildLanguages()
        {
            Program.WriteLine("[Builder] Building language files...");
            foreach (var data in _languages)
            {
                BuildLanguage(data.Key, data.Value);
            }
            Program.WriteLine("[Builder] Language files has been succesfully builded.");
        }

        private void BuildLanguage(string langId, Dictionary<string, string> phrases)
        {
            Program.WriteLine($"[Builder] Start working with locale {langId}...");
            var RootKV = new KeyValue("Phrases");

            foreach (var Phrase in phrases)
            {
                var PhraseKV = new KeyValue(Phrase.Key);
                PhraseKV.Children.Add(new KeyValue(langId, Phrase.Value));

                Program.WriteLine($"[Builder] Added phrase {Phrase.Key} -> {Phrase.Value}...");
                RootKV.Children.Add(PhraseKV);
            }

            var dirPath = $"{_opts.Output}/{langId}";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            RootKV.SaveToFile($"{dirPath}/{_opts.FileName}.phrases.txt", false);
            Program.WriteLine($"[Builder] Language {langId} saved.");
        }

        /// <summary>
        /// Builders for Format Rules
        /// </summary>
        private void BuildFormatRules()
        {
            Program.WriteLine("[Builder] Start working with Format Rules...");
            var rules = new List<KeyValue>();
            foreach (var data in _formatRules)
            {
                var formatKv = new KeyValue("#format", BuildFormatRule(data.Value));
                var kv = new KeyValue(data.Key);

                Program.WriteLine($"[Builder] Parsed format rule for phrase {data.Key}");
                kv.Children.Add(formatKv);

                rules.Add(kv);
            }

            var rootKv = new KeyValue("Phrases");
            foreach (var rule in rules)
            {
                rootKv.Children.Add(rule);
            }

            rootKv.SaveToFile($"{_opts.Output}/{_opts.FileName}.phrases.txt", false);
        }

        private string BuildFormatRule(List<string> rules)
        {
            var preparedRules = new List<string>();
            var id = 1;
            var type = ' ';
            foreach (var rule in rules)
            {
                switch (rule.ToLower())
                {
                    case "string":
                        type = 's';
                        break;

                    case "integer":
                        type = 'i';
                        break;

                    case "decimal":
                        type = 'd';
                        break;

                    case "binary":
                        type = 'x';
                        break;

                    case "float":
                        type = 'f';
                        break;

                    case "name":
                        type = 'N';
                        break;

                    case "log":
                        type = 'L';
                        break;

                    case "phrase":
                        type = 't';
                        break;

                    default:
                        Program.WriteLine($"[FORMAT BUILDER] Unknown rule type {rule}");
                        continue;
                }

                preparedRules.Add($"{{{id++}:{type}}}");
            }

            return preparedRules.ToDelimitedString(",");
        }
    }
}