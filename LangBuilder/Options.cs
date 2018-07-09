using CommandLine;

namespace LangBuilder
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input files to be processed")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Directory where translations should be created")]
        public string Output { get; set; }

        [Option('n', "name", Required = true, HelpText = "Name for phrases file")]
        public string FileName { get; set; }

        // [Option('w', "watermark", Required = false, Default = true, HelpText = "Manipulates the program watermark in translations file")]
        // public bool Watermark { get; set; }
    }
}