using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Localization.Tools.Extensions;

internal static class SemanticModelExtensions
{
    internal static bool IsAttributeOfType(this SemanticModel semanticModel, AttributeSyntax attributeSyntax, Type toEvaluate) => semanticModel.GetTypeInfo(attributeSyntax).ConvertedType?.ToString().EqualsOrDefault(toEvaluate.ToString()) ?? false;
    
}
