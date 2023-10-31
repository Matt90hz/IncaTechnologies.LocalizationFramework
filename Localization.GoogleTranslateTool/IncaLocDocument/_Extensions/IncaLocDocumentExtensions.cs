using Localization.GoogleTranslateTool.__Exception;
using Localization.GoogleTranslateTool.IncaLocaDocument;
using System.Globalization;
using System.Xml.Linq;

namespace Localization.GoogleTranslateTool.IncaLocaDocument._Extensions
{
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

                var localizations = baseElement
                    .Descendants(IncaLocTags.LOCALIZE_ELEMENT)
                    .Select(localizeElement =>
                    {
                        var property = localizeElement.Attribute(IncaLocTags.PROPERTY_ATTRIBUTE)?.Value
                            ?? throw new($"Localize element do not have a property attribute.");

                        var cultures = localizeElement
                            .Elements()
                            .Select(element => new Translation(new CultureInfo(element.Name.LocalName), element.Value));

                        return new Property(property, cultures);
                    });

                return new IncaLocDocument(nameSpace, @class, localizations);
            }
            catch (Exception e)
            {
                return new($"Filed to parse xml document in {nameof(IncaLocDocument)}.", e);
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

            foreach (var culture in property.Translations)
            {
                localizeElement.Add(culture.ToXElement());
            }

            return localizeElement;
        }

        public static XElement ToXElement(this Translation culture) => new(culture.CultureInfo.Name, culture.Text);

        public static Translation? FindTranslation(this Property property, CultureInfo cultureInfo) =>
            property.Translations.FirstOrDefault(culture => culture.CultureInfo.Equals(cultureInfo));

        public static bool IsTranslated(this Translation translation) =>
            !string.IsNullOrWhiteSpace(translation.Text);
    }
}
