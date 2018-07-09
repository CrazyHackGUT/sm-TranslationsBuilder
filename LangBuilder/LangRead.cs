using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace LangBuilder
{
    public class LangRead
    {
        private readonly Options _opts;
        private Dictionary<string, List<string>> _formatRules;
        private Dictionary<string, Dictionary<string, string>> _languages;

        public LangRead(Options opts)
        {
            _opts = opts;
            _formatRules = new Dictionary<string, List<string>>();
            _languages = new Dictionary<string, Dictionary<string, string>>();
        }

        public void Run()
        {
            ProcessInput();
            ProcessOutput();
        }

        private void ProcessInput()
        {
            Program.WriteLine("Trying load format rules... ");
            try
            {
                LoadFormatRules();
            }
            catch
            {}

            foreach (var entry in Directory.EnumerateFiles(_opts.Input))
            {
                if (!entry.EndsWith("format_rules.json"))
                {
                    var dummy = entry.Replace(".json", "").Replace("\\", "/").Split("/");
                    var Lang = dummy[dummy.Length - 1];

                    Program.WriteLine($"Trying load lang file {Lang}... ");
                    try
                    {
                        LoadLang(Lang);
                    }
                    catch
                    {}
                }
            }
        }

        private void ProcessOutput()
        {
            // What languages we have?
            Program.WriteLine("[Report] Processing loaded data...");
            foreach (var langId in _languages.Keys)
            {
                Program.WriteLine($"[Report] Loaded {langId} language.");
            }

            foreach (var formatId in _formatRules.Keys)
            {
                Program.WriteLine($"[Report] Loaded format rules for phrase {formatId}");
            }

            // Building...
            new LangBuild(_opts, _formatRules, _languages).Run();
        }

        /// <summary>
        /// Loaders
        /// </summary>
        private void LoadFormatRules()
        {
            var path = $"{_opts.Input}/format_rules.json";
            if (!File.Exists(path))
                throw new Exception("File not found");

            var data = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));

            IList<string> keys = data.Properties().Select(p => p.Name).ToList();
            foreach (var key in keys)
            {
                Program.WriteLine($"[FormatRules] Found format rules for phrase {key}");

                var value = data[key];
                var list = new List<string>();
                foreach (var type in value)
                {
                    list.Add(type.Value<string>());
                }

                _formatRules.Add(key, list);
            }
        }

        private void LoadLang(string LangID)
        {
            var path = $"{_opts.Input}/{LangID}.json";
            if (!File.Exists((path)))
                throw new Exception("File not found");

            var data = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));

            IList<string> keys = data.Properties().Select(p => p.Name).ToList();
            var phrases = new Dictionary<string, string>();

            foreach (var key in keys)
            {
                Program.WriteLine($"[{LangID}] Found phrase {key}");

                var value = data[key].Value<string>();
                phrases.Add(key, value);
            }

            _languages.Add(LangID, phrases);
        }
    }
}