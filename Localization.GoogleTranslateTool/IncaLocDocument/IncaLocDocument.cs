using System.Globalization;

namespace Localization.GoogleTranslateTool.IncaLocaDocument
{
    internal sealed record IncaLocDocument(string NameSpace, string Class, IEnumerable<Property> Properties);

    internal sealed record Property(string Name, IEnumerable<Translation> Translations);

    internal sealed record Translation(CultureInfo CultureInfo, string Text);

}
