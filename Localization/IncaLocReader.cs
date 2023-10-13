using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Transactions;
using System.Xml.Linq;
using Localization.ExceptionResult;
using Localization.Extensions;
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
        public virtual CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <inheritdoc/>
        public virtual CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <inheritdoc/>
        public virtual string GetText(IncaLocParameters parameters)
        {
            var assembly = Assembly.GetCallingAssembly();
            var localizeElement = IncaLocFile(parameters, assembly).DescendantWithAttribute(parameters.PropertyIdentifier);

            //Return the translation
            return TextAsCurrentCulture() 
                ?? TextAsCurrentLanguage() 
                ?? TextAsDefaultCulture() 
                ?? TextAsDefaultLanguage() 
                ?? TextAsAnyLanguage()
                ?? string.Empty;

            string? TextAsCurrentCulture() => localizeElement?.Element(CurrentCulture.Name)?.Value;
            string? TextAsDefaultCulture() => localizeElement?.Element(DefaultCulture.Name)?.Value;
            string? TextAsCurrentLanguage() => localizeElement?.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == CurrentCulture.Name[..2])?.Value;
            string? TextAsDefaultLanguage() => localizeElement?.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == DefaultCulture.Name[..2])?.Value;
            string? TextAsAnyLanguage() => localizeElement?.Elements().FirstOrDefault()?.Value;
        }

        static XDocument IncaLocFile(IncaLocParameters incaLocParameters, Assembly assembly) 
        {
            var resourceName = IncaLocResourceName(incaLocParameters, assembly).Value();
            var stream = assembly.GetManifestResourceStream(resourceName);

            var xml = XDocument.Load(stream);

            //Manually dispose of the stream just in case
            stream.Dispose();

            return xml;
        }

        static Exception<string> IncaLocResourceName(IncaLocParameters incaLocParameters, Assembly assembly)
        {
            var resourceName = assembly.FindManifestResourceName(incaLocParameters.FileName());

            if (resourceName is null) return new Exception<string>($"Resource {incaLocParameters.FileName()} not found!");

            return resourceName;
        }
    }

}
