using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using Localization.Interfaces;

namespace Localization
{
    /// <summary>
    /// Object to read .incaloc files
    /// </summary>
    public class IncaLocReader : IncaLocBase, IIncaLocReader
    {
        /// <summary>
        /// Default instance of <see cref="IncaLocReader"/>.
        /// </summary>
        public static IncaLocReader Default { get; } = new IncaLocReader();

        /// <inheritdoc/>
        public virtual CultureInfo CurrentCulture { get; set; } = Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public virtual CultureInfo DefaultCulture { get; set; } = Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public virtual string GetText(IncaLocParameters parameters)
        {
            //Retrive the resource
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(r => r.Contains(GetFileName(parameters)))) ??
                throw new FileNotFoundException($"{GetFileName(parameters)} not found!");

            //Create XDocument
            var xml = XDocument.Load(stream);

            //Manually dispose of the stream just in case
            stream.Dispose();

            //return the current culture localized text or the default one if is not found.
            return xml
                .Element(BASE_ELEMENT)
                .Elements(LOCALIZE_ELEMENT)
                .FirstOrDefault(le => le.Attribute(PROPERTY_ATTRIBUTE).Value == parameters.PropertyIdentifier) is XElement property ? 
                (property.Element(CurrentCulture.Name) ?? property.Element(DefaultCulture.Name)) is XElement culture ? 
                culture.Value.Trim().Replace('\t', '\0') : null : null;
        }
    }

}
