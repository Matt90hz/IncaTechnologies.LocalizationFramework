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
using Localization.Tools.ExeptionResult;

namespace Localization.Tools;

/// <summary>
/// Parse the arguments passed to the <see cref="Main(string[])"/> method.
/// </summary>
public class Options
{
    public const string DefaultGenerator = "DEFAULT_GENERATOR";
    public const string CSVGenerator = "CSV_GENERATOR";

    [Option('v', "verbose", Required = false, HelpText = "Show verbose messages.")]
    public bool Verbose { get; set; }

    [Option("diagnostic", Required = false, HelpText = "Show diagnostic messages.")]
    public bool Diagnostic { get; set; }

    [Option('i', "input", Required = false, HelpText = "Project folder.")]
    public string Input { get; set; } = DefautInput.ToString();

    [Option('o', "output", Required = false, HelpText = "Where the localization file will be stored.")]
    public string Output { get; set; } = DefautOuput.ToString();

    [Option('g', "generator", Required = false, HelpText = "Specify a different custom file generator.")]
    public string IncaLocGenerator { get; set; } = DefaultGenerator;

    [Option('c', "cultures", Required = false, Separator = ',', HelpText = "All the culture codes separated by commas (ex: -c \"en-EN, fr-FR, es-ES\")")]
    public IEnumerable<string> Cultures { get; set; } = new[] { Thread.CurrentThread.CurrentCulture.Name };

    public static Uri DefautOuput => new (Path.Combine(Environment.CurrentDirectory, @"Localization"));
    public static Uri DefautInput => new (Environment.CurrentDirectory);

}

internal static class OptionsExtensions
{
    internal static Exception<IIncaLocGenerator> Generator(this Options options)
    {
        try
        {
            return options.IncaLocGenerator switch
            {
                _ => new IncaLocGenerator(
                        options.ValidateOutput().Value(),
                        options.ValidateInput().Value().FileWithExtension("csproj")!,
                        options.Cultures.Select(c => new CultureInfo(c.Trim())))
            };
        }
        catch (Exception<Uri> e)
        {
            return new Exception<IIncaLocGenerator>("Failed to create a generator.", e);
        }
    }

    internal static Exception<Uri> ValidateInput(this Options options)
    {
        try
        {
            var uri = new Uri(options.Input);

            if (!uri.UriContainsCsproj()) return new Exception<Uri>("Invalid Input. The Input path must contain a .csproj file.");
               
            return uri;
        }
        catch (Exception e)
        {       
            return new Exception<Uri>("Invalid Input.", e);
        }
    }

    internal static Exception<Uri> ValidateOutput(this Options options, Action<Exception>? log = null)
    {
        try
        {
            var uri = new Uri(options.Output);

            return uri;
        }
        catch (Exception e)
        {
            if (log is not null) log(e);
            return new Exception<Uri>("Invalid Output.", e);
        }
    }

}

internal class Program
{
    private static readonly IEnumerable<PortableExecutableReference> _defaultPortableExecutableReferences = new PortableExecutableReference[]
    {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(IncaLocalizeAttribute).Assembly.Location),
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
        try
        {
            var generator = options.Generator().Value();
            var dotCsfiles = new Uri(options.Input).DotCsFiles();

            foreach (var file in dotCsfiles)
            {
                if (options.Verbose) Log($"Analyzing file: {file}");

                GenerateIncaLocFile(file, generator, options);

                if (options.Verbose) Log(string.Empty);
            }
        }
        catch(Exception<IIncaLocGenerator> e)
        {
            Console.WriteLine(e);
            return;
        }
    }

    static void GenerateIncaLocFile(Uri uri, IIncaLocGenerator generator, Options options)
    {
        var file = uri.ReadFile().Value();
        var attributes = LocalizeAttributes(file, options);

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
                if (options.Verbose) Log($"Failed to generate .incaloc for {param}.\n{e}\n");
            }
        }
    }

    static IEnumerable<AttributeSyntax> LocalizeAttributes(string file, Options options)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(file);
        var compilation = _compilation.AddSyntaxTrees(syntaxTree);
        if (options.Diagnostic) WriteDiagnostic(compilation);

        var localizeAttributes = syntaxTree.FindAttributeOfType<IncaLocalizeAttribute>(compilation);

        if (options.Verbose && localizeAttributes.IsEmpty()) Log($"Not found properties decorated with localize attribute.");

        return localizeAttributes;
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