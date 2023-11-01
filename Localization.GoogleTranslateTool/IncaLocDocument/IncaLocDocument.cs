using Localization.GoogleTranslateTool.__Exception;
using System.Globalization;
using System.Xml.Linq;

namespace Localization.GoogleTranslateTool.IncaLocaDocument;

internal sealed record IncaLocDocument(string NameSpace, string Class, IEnumerable<Property> Properties);

internal sealed record Property(string Name, IEnumerable<Translation> Translations);

internal sealed record Translation(CultureInfo CultureInfo, string Text);

internal static class IncaLocDocumentExtensions
{
    public static Exception<IncaLocDocument> ToIncaLocDocument(this XDocument document)
    {
        try
        {
            var baseElement = document.Element(IncaLocTags.BASE_ELEMENT)
                 ?? throw new($"The docuemnt do not have {IncaLocTags.BASE_ELEMENT} element.");

            var nameSpace = baseElement.Attribute(IncaLocTags.NAME_SPACE_ATTRIBUTE)?.Value
                ?? throw new($"The {IncaLocTags.BASE_ELEMENT} element do not have a {IncaLocTags.NAME_SPACE_ATTRIBUTE} attribute.");

            var @class = baseElement.Attribute(IncaLocTags.CLASS_ATTRIBUTE)?.Value
                ?? throw new($"The {IncaLocTags.BASE_ELEMENT} element do not have a {IncaLocTags.CLASS_ATTRIBUTE} attribute.");

            var properties = baseElement
                .Descendants(IncaLocTags.LOCALIZE_ELEMENT)
                .Select(localizeElement =>
                {
                    var property = localizeElement.Attribute(IncaLocTags.PROPERTY_ATTRIBUTE)?.Value
                        ?? throw new($"Localize element do not have a property attribute.");

                    var transalations = localizeElement
                        .Elements()
                        .Select(element => new Translation(new CultureInfo(element.Name.LocalName), element.Value));

                    return new Property(property, transalations);
                });

            return new IncaLocDocument(nameSpace, @class, properties);
        }
        catch (Exception e)
        {
            return new($"Filed to parse xml document to {nameof(IncaLocDocument)}.", e);
        }
    }

    public static XDocument ToXDocument(this IncaLocDocument incaLocDocument)
    {
        var document = new XDocument(new XDeclaration("1.0", "utf-8", null));

        var baseElement = new XElement(
            IncaLocTags.BASE_ELEMENT,
            new XAttribute(IncaLocTags.NAME_SPACE_ATTRIBUTE, incaLocDocument.NameSpace),
            new XAttribute(IncaLocTags.CLASS_ATTRIBUTE, incaLocDocument.Class));

        document.Add(baseElement);

        foreach (var property in incaLocDocument.Properties)
        {
            baseElement.Add(property.ToXElement());
        }

        return document;
    }

    public static XElement ToXElement(this Property property)
    {
        var localizeElement = new XElement(IncaLocTags.LOCALIZE_ELEMENT, new XAttribute(IncaLocTags.PROPERTY_ATTRIBUTE, property.Name));

        foreach (var translation in property.Translations)
        {
            localizeElement.Add(translation.ToXElement());
        }

        return localizeElement;
    }

    public static XElement ToXElement(this Translation transaltion) => new(transaltion.CultureInfo.Name, transaltion.Text);

    public static Translation? FindTranslation(this Property property, CultureInfo cultureInfo) =>
        property.Translations.FirstOrDefault(culture => culture.CultureInfo.Equals(cultureInfo));

    public static bool IsTranslated(this Translation translation) => !string.IsNullOrWhiteSpace(translation.Text);
}
