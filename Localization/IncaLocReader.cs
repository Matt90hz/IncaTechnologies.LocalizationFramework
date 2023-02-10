using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        /// <inheritdoc/>
        public CultureInfo CurrentCulture { get; set; } = new CultureInfo("en-EN"); //Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("es-ES"); // Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public string GetText(IncaLocParameters parameters)
        {
            //Retrive the resource
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(r => r.Contains(GetFileName(parameters)))) ??
                throw new FileNotFoundException($"{GetFileName(parameters)} not found!");

            //Create XDocument
            var xml = XDocument.Load(stream);

            //Manually dispose of the stream just in case
            stream.Dispose();

            //Send back the value
            return xml.Element(BASE_ELEMENT).Elements(LOCALIZE_ELEMENT)
                .FirstOrDefault(le => le.Attribute(PROPERTY_ATTRIBUTE).Value == parameters.PropertyIdentifier)?
                .Element(CurrentCulture.Name)?.Value.Trim().Replace('\t', '\0');
        }
    }

}
