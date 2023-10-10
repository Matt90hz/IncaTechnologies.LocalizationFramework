using CommandLine;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Text;
using Localization.Tools.Extensions;
using Localization.Interfaces;
using System.Globalization;
using System.Reflection.Emit;

namespace Localization.Tools;

/// <summary>
/// Parse the arguments passed to the <see cref="Main(string[])"/> method.
/// </summary>
public class Options
{
    public const string DefaultGenerator = "DEFAULT_GENERATOR";

    [Option('v', "verbose", Required = false, HelpText = "Show verbose messages.")]
    public bool Verbose { get; set; }

    [Option("diagnostic", Required = false, HelpText = "Show diagnostic messages.")]
    public bool Diagnostic { get; set; }

    [Option('i', "input", Required = false, HelpText = "Project folder.")]
    public string Input { get; set; } = Environment.CurrentDirectory;

    [Option('o', "output", Required = false, HelpText = "Where the localization file will be stored.")]
    public string Output { get; set; } = Path.Combine(Environment.CurrentDirectory, @"Localization");

    [Option('g', "generator", Required = false, HelpText = "Specify a different custom file generator.")]
    public string IncaLocGenerator { get; set; } = DefaultGenerator;

    [Option('c', "cultures", Required = false, Separator = ',', HelpText = "All the culture codes separated by commas (ex: -c \"en-EN, fr-FR, es-ES\")")]
    public IEnumerable<string> Cultures { get; set; } = new[] { Thread.CurrentThread.CurrentCulture.Name };

}

internal static class OptionsExtensions
{
    private static readonly Action<Exception> _log = e => Console.WriteLine(e.Message);

    internal static IIncaLocGenerator GetGenerator(this Options options) => options.IncaLocGenerator switch
    {
        _ => new IncaLocGenerator(
            storeLocation: options.Output,
            projectFile: Directory.GetFiles(options.Input).First(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)),
            cultureInfos: options.Cultures.Select(c => new CultureInfo(c.Trim())))
    };

    internal static Uri? ValidateInput(this Options options, Action<Exception>? log = null)
    {
        log ??= _log;

        Uri uri;
        try
        {
            uri = new Uri(options.Input);

            if (!uri.UriContainsCsproj())
            {
                log(new Exception("Invalid Input. The Input path must contain a .csproj file."));
                return null;
            }

            return uri;
        }
        catch (Exception e)
        {
            log(e);
            return null;
        }
    }

    internal static Uri? ValidateOutput(this Options options, Action<Exception>? log = null)
    {
        log ??= _log;

        Uri uri;
        try
        {
            uri = new Uri(options.Output);
            return uri;
        }
        catch (Exception e)
        {
            log(e);
            return null;
        }
    }
}

internal class Program
{
    private static readonly IEnumerable<PortableExecutableReference> _defaultPortableExecutableReferences = new PortableExecutableReference[]
    {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(IncaLocGenerator).Assembly.Location),
    };

    private static readonly CSharpCompilation _compilation = CSharpCompilation.Create("NotRelevant")
        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        .AddReferences(_defaultPortableExecutableReferences);

    static void Main(string[] args) => Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(GenerateIncaLocFiles);

    static void GenerateIncaLocFiles(Options options)
    {
        if (options.ValidateInput() is not Uri inputUri) return;
        if (options.ValidateOutput() is null) return;

        GenerateIncaLocFiles(inputUri, options);
    }

    static void GenerateIncaLocFiles(Uri input, Options options)
    {
        var files = input.GetDotCsFiles();
        var generator = options.GetGenerator();

        foreach (var file in files)
        {
            if (options.Verbose) Log($"Analyzing file: {file}");

            GenerateIncaLocFile(file, generator, options);
        }
    }

    static void GenerateIncaLocFile(Uri uri, IIncaLocGenerator generator, Options options) => WriteIncaLocFile(uri.TryReadFile(), generator, options);     

    static void WriteIncaLocFile(string file, IIncaLocGenerator generator, Options options)
    {
        var attributes = GetLocalizeAttributes(file, options);

        foreach (var attribute in attributes)
        {
            var param = attribute.ToIncaLocParameters();

            try
            {
                generator.Generate(param);

                if (options.Verbose) Log($"Generated .incaloc for {param}.");
            }
            catch (Exception e)
            {
                if (options.Verbose) Log($"Failed to generate .incaloc for {param}.\n{e.Message}\n");
            }
        }
    }

    static IEnumerable<AttributeSyntax> GetLocalizeAttributes(string file, Options options)
    {
        if (string.IsNullOrEmpty(file)) return Enumerable.Empty<AttributeSyntax>();

        var syntaxTree = CSharpSyntaxTree.ParseText(file);

        return FindLocalizeAttributes(syntaxTree, options).LogIfEmpty("No properties are decorated with IncaLocalizeAttribute.");
    }

    static IEnumerable<AttributeSyntax> FindLocalizeAttributes(SyntaxTree syntaxTree, Options options)
    {
        var compilation = _compilation.AddSyntaxTrees(syntaxTree);

        if (options.Diagnostic) WriteDiagnostic(compilation);

        return syntaxTree.FindAttributeOfType(compilation, typeof(IncaLocalizeAttribute));
    }

    static void WriteDiagnostic(Compilation compilation)
    {
        Log("DIAGNOSTIC");
        Log(compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.GetMessage())
            .Aggregate("", (result, next) => result + next + "\n"));

        Log("END DIAGNOSTIC\n");

    }

    static void Log(string message) => Console.WriteLine(message);
}