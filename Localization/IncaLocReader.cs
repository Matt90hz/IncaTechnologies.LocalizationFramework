using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Transactions;
using System.Xml.Linq;
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
        public virtual CultureInfo CurrentCulture { get; set; } = Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public virtual CultureInfo DefaultCulture { get; set; } = Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public virtual string GetText(IncaLocParameters parameters)
        {
            //Get the element to localize
            var localizeElement = IncaLocFile(parameters).DescendantWithAttribute(parameters.PropertyIdentifier);

            //Return the translation
            return 
                localizeElement is null ? string.Empty :
                Translation1() is string t1 ? t1 :
                Translation2() is string t2 ? t2 :
                Translation3() is string t3 ? t3 :
                Translation4() is string t4 ? t4 :
                string.Empty;

            string? Translation1() => localizeElement.Element(CurrentCulture.Name)?.Value;
            string? Translation2() => localizeElement.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == CurrentCulture.Name[..2])?.Value;
            string? Translation3() => localizeElement.Element(DefaultCulture.Name)?.Value;
            string? Translation4() => localizeElement.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == DefaultCulture.Name[..2])?.Value;
        }

        static XDocument IncaLocFile(IncaLocParameters incaLocParameters) 
        {
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(r => r.Contains(incaLocParameters.FileName()));
            var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"{incaLocParameters.FileName()} not found!");

            var xml = XDocument.Load(stream);

            //Manually dispose of the stream just in case
            stream.Dispose();

            return xml;
        }
    }

}
