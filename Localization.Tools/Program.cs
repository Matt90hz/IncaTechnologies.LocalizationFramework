using CommandLine;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Text;
using Localization.Interfaces;
using System.Globalization;

namespace Localization.Tools
{
    internal class Program
    {
        /// <summary>
        /// Parse the arguments passed to the <see cref="Main(string[])"/> method.
        /// </summary>
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Show verbose messages.")]
            public bool Verbose { get; set; }

            [Option("diagnostic", Required = false, HelpText = "Show diagnostic messages.")]
            public bool Diagnostic { get; set; }

            [Option('i', "input", Required = false, HelpText = "Project folder.")]
            public string Input { get; set; } = Environment.CurrentDirectory;

            [Option('o', "output", Required = false, HelpText = "Where the localization file will be stored.")]
            public string Output { get; set; } = Path.Combine(Environment.CurrentDirectory, @"Localization");

            [Option('g', "generator", Required = false, HelpText = "Specify a different custom file generator.")]
            public IIncaLocGenerator? IncaLocGenerator { get; set; }

            [Option('c', "cultures", Required = false, Separator = ',', HelpText = "All the culture codes separated by commas (ex: -c \"en-EN, fr-FR, es-ES\")")]
            public IEnumerable<string>? Cultures { get; set; }

            public bool IsValidInput => Directory.GetFiles(Input).Any(f => f.EndsWith(".csproj"));

            public bool IsValidOutput => Uri.TryCreate(Output, UriKind.Absolute, out _);

        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Parse args
            Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
        }

        /// <summary>
        /// Runned if argument parsing succeed.
        /// </summary>
        /// <param name="opts"></param>
        static void RunOptions(Options opts)
        {
            //Check if the current directory is a project directory
            if (!opts.IsValidInput)
            { 
                Console.WriteLine("Invalid Input. The Input path must contain a .csproj file.");
                return;
            }

            //Check if output path is valid
            if (!opts.IsValidOutput)
            {
                Console.WriteLine("Invalid Output. The Output path is not a valid uri.");
                return;
            }

            //VERBOSE
            if (opts.Verbose) 
            { 
                Console.WriteLine($"Input path: {opts.Input}");
                Console.WriteLine($"Output path: {opts.Output}\n");
            }

            //IncaLocGenerator
            var incaloc = opts.IncaLocGenerator ?? new IncaLocGenerator(
                storeLocation: opts.Output,
                projectFile: Directory.GetFiles(opts.Input).First(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)),
                cultureInfos: opts.Cultures?.Any() ?? false ? opts.Cultures.Select(c => new CultureInfo(c.Trim())) : null);

            //Get all the .cs file in the Input directory
            var csFiles = Directory.GetFiles(opts.Input, "*.cs", SearchOption.AllDirectories);       

            //Look for LocalizeAttribute in each .cs file
            foreach (var csFile in csFiles)
            {
                GenerateIncalocFile(csFile, incaloc, opts.Verbose, opts.Diagnostic);
            }

            //Inform that the task is completed
            Console.WriteLine("Localization files generated.");
        }

        /// <summary>
        /// Generates .incaloc Files based on the presence of an <see cref="IncaLocalizeAttribute"/> in the classes contained in the <paramref name="csFile"/>.
        /// </summary>
        /// <param name="csFile">Path of the cs file to analyze</param>
        /// <param name="incaLocGenerator">Generator for .incaloc files</param>
        /// <param name="verbose">Show verbose messages to the console</param>
        /// <param name="diagnostic">Show diagnostic message to the console</param>
        static void GenerateIncalocFile(string csFile, IIncaLocGenerator incaLocGenerator, bool verbose, bool diagnostic)
        {
            //VERBOSE
            if (verbose) Console.WriteLine($"Analyze file: {csFile}");

            //Parse text into SyntaxTree
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(csFile));

            //Get all properties decorated with attributes
            var attributes = tree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Where(p => p.AttributeLists.Any())
                .Select(p => p.AttributeLists.SelectMany(al => al.Attributes)).SelectMany(a => a)
                .ToArray();

            //No properties decorated with attributes found
            if (!attributes.Any())
            {
                //VERBOSE
                if (verbose) Console.WriteLine("None of the properties is decorated with attributes.\n");
                return;
            }

            //Create a compiled program form the syntax tree
            var compilation = CSharpCompilation.Create("NotRelevant")
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(PortableExecutableReferenceProvider.PortableExecutableReferences)
                    .AddSyntaxTrees(tree);
            
            //Compile the SyntaxTree for semantic analisys
            var model = compilation.GetSemanticModel(tree);

            //DIAGNOSTIC
            if (diagnostic) WriteDiagnostic(model.Compilation);

            //Get all attributes that are of Type LocalizationAttribute
            var localizeAttributes = attributes.Where(a => model.GetTypeInfo(a).Type?.Name == nameof(Localization.IncaLocalizeAttribute)).ToArray();

            //No properties decorated with LocalizeAttribute found
            if (!localizeAttributes.Any())
            {
                //VERBOSE
                if (verbose) Console.WriteLine("None of the properties is decorated with LocalizeAttribute.\n");
                return;
            }

            //Project attributes in IncaLocGeneratorParamenters
            var incalocParams = localizeAttributes.Select(la => new IncaLocParameters(
                nameSpace: la.Ancestors().OfType<NamespaceDeclarationSyntax>().First().Name.ToString(),
                classIdentifier: la.Ancestors().OfType<ClassDeclarationSyntax>().First().Identifier.Text,
                propertyIdentifier: la.Ancestors().OfType<PropertyDeclarationSyntax>().First().Identifier.Text))
                .ToArray();

            //Generate incaloc files
            incaLocGenerator.Generate(incalocParams);

            //VERBOSE
            if (verbose)
            {
                Console.WriteLine("Generated a .incaloc file for the following properties:");
                Console.WriteLine(incalocParams
                    .Select(ip => string.Join(".", ip.NameSpace, ip.ClassIdentifier, ip.PropertyIdentifier))
                    .Aggregate("", (result, next) => result + next + "\n"));
            }
        }

        static void WriteDiagnostic(Compilation compilation)
        {           
            Console.WriteLine("DIAGNOSTIC");
            Console.WriteLine(compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.GetMessage())
                .Aggregate("", (result, next) => result + next + "\n"));

            Console.WriteLine("END DIAGNOSTIC\n");
            
        }



    }
    
}