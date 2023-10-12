using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Localization.Extensions
{
    internal static class XContainerExtensions
    {
        internal static XDocument Document(this XContainer container) => container.Document;

        internal static TXContainer AddElement<TXContainer>(this TXContainer container, XElement element)
            where TXContainer : XContainer
        {
            container.Add(element);

            return container;
        }

        internal static TXContainer AddElements<TXContainer>(this TXContainer container, IEnumerable<XElement> elements)
            where TXContainer : XContainer
        {
            foreach (var element in elements)
            {
                container.Add(element);
            }

            return container;
        }

        internal static TXContainer AddNode<TXContainer>(this TXContainer container, XNode element)
            where TXContainer : XContainer
        {
            container.Add(element);

            return container;
        }

        internal static XElement? DescendantWithAttribute(this XContainer container, XName name) => container
            .Descendants()
            .SelectMany(e => e.Attributes())
            .FirstOrDefault(a => a.Name.Equals(name))
            ?.Parent;

        internal static XElement? DescendantWithAttribute(this XContainer container, string value) => container
            .Descendants()
            .SelectMany(e => e.Attributes())
            .FirstOrDefault(a => a.Value.Equals(value))
            ?.Parent;

        internal static bool HasDescendantWithAttribute(this XContainer container, XName name) => container
            .Descendants()
            .SelectMany(e => e.Attributes())
            .Any(a => a.Name.Equals(name));

        internal static bool HasDescendantWithAttribute(this XContainer container, string value) => container
            .Descendants()
            .SelectMany(e => e.Attributes())
            .Any(a => a.Value.Equals(value));

        internal static XElement Descendant(this XContainer container, XName name) => container.Descendants(name).FirstOrDefault();
    }

}