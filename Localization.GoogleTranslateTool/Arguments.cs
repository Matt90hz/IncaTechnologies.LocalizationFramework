
using CommandLine;

class Arguments
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('i', "input", Required = false, HelpText = "File or directory to be processed.")]
    public string IncaLocFilePath { get; set; } = Environment.CurrentDirectory;

    [Option('f', "from", Required = true, HelpText = "Culture to translate from.")]
    public string TranslateFrom { get; set; } = string.Empty;
}