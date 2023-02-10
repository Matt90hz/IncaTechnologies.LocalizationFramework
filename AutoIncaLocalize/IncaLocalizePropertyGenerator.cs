using Localization;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace AutoIncaLocalize
{
    [Generator]
    public class IncaLocalizePropertyGenerator : ISourceGenerator
    {
       
        public void Execute(GeneratorExecutionContext context)
        {
            //Retrive the resource
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(r => r.Contains("HelloGenerated"))) ??
                throw new FileNotFoundException($"File not found!");

            var sourceText = SourceText.From(stream, Encoding.UTF8);

            context.AddSource("HelloGenerated.g.cs", sourceText);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }


    }
}
