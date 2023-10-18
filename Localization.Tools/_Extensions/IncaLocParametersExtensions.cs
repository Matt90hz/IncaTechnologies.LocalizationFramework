using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Localization.Tools.Extensions;

internal static class IncaLocParametersExtensions
{
    internal static IncaLocParameters ToIncaLocParameters(this AttributeSyntax attribute) => new(attribute.ContainingNamespaceName(), attribute.ContaingClassName(), attribute.DecoratedPropertyName());
}
