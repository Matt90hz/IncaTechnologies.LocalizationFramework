using Localization.ExceptionResult;
using Localization.Extensions;
using Localization.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Localization
{
    /// <summary>
    /// Extension methods for <see cref="IncaLocReader"/>.
    /// </summary>
    public static class IncaLocReaderExtesions
    {
        /// <summary>
        /// Get the localized text from the .incaloc file using the default <see cref="IIncaLocReader"/>. 
        /// It uses the name of the caller as identifier. If nothing is found the alternative is returned.
        /// </summary>
        /// <param name="toTranslate"></param>
        /// <param name="propertyName"></param>
        /// <param name="incaLocReader"></param>
        /// <returns>Localized text or the name of the property to localize.</returns>
        public static string GetTextOrPropertyName(
            this object toTranslate,
            [CallerMemberName] string propertyName = "",
            IIncaLocReader? incaLocReader = null)
            => toTranslate.GetTextOr(propertyName, propertyName, incaLocReader);

        /// <summary>
        /// Get the localized text from the .incaloc file using the default <see cref="IIncaLocReader"/>. 
        /// It uses the name of the caller as identifier. If nothing is found the alternative is returned.
        /// </summary>
        /// <param name="toTranslate"></param>
        /// <param name="alternative">Alternative text</param>
        /// <param name="propertyName"></param>
        /// <param name="incaLocReader"></param>
        /// <returns>Localized text or <paramref name="alternative"/>.</returns>
        public static string GetTextOr(
            this object toTranslate,
            string alternative,
            [CallerMemberName] string propertyName = "",
            IIncaLocReader? incaLocReader = null)
            => toTranslate.GetText(propertyName, incaLocReader) is string translated
            && string.IsNullOrEmpty(translated) is false
            ? translated
            : alternative;

        /// <summary>
        /// Get the localized text from the .incaloc file using the default <see cref="IIncaLocReader"/>. 
        /// It uses the name of the caller as identifier.
        /// </summary>
        /// <param name="toTranslate">Used just to extract the type</param>
        /// <param name="propertyName"></param>
        /// <param name="incaLocReader"></param>
        /// <returns>Translated text or an empty string if nothing as been found.</returns>
        public static string GetText(
            this object toTranslate,
            [CallerMemberName] string propertyName = "",
            IIncaLocReader? incaLocReader = null)
        {
            var parameters = toTranslate.GetParameters(propertyName);
            var assembly = Assembly.GetAssembly(toTranslate.GetType());

            return (incaLocReader ?? IncaLocReader.Default).GetText(parameters, assembly);
        }

        internal static string GetText(this IIncaLocReader incaLocReader, IncaLocParameters parameters, Assembly assembly)
            => assembly.GetEmbeddedIncaLocFile(parameters) is XDocument incaLocFile
            ?  incaLocReader.GetText(parameters, incaLocFile)
            : string.Empty;

        internal static string GetText(this IIncaLocReader incaLocReader, IncaLocParameters parameters, XDocument incaLocFile)
            => incaLocFile.DescendantWithAttribute(parameters.PropertyIdentifier) is XElement localizeElement
            ? incaLocReader.GetText(localizeElement)
            : string.Empty;

        internal static string GetText(this IIncaLocReader incaLocReader, XElement localizeElement)
        {
            return TextAsCurrentCulture()
                ?? TextAsCurrentLanguage()
                ?? TextAsDefaultCulture()
                ?? TextAsDefaultLanguage()
                ?? TextAsAnyLanguage()
                ?? string.Empty;

            string? TextAsCurrentCulture() => localizeElement.Element(incaLocReader.CurrentCulture.Name)?.Value;
            string? TextAsDefaultCulture() => localizeElement.Element(incaLocReader.DefaultCulture.Name)?.Value;
            string? TextAsCurrentLanguage() => localizeElement.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == incaLocReader.CurrentCulture.Name[..2])?.Value;
            string? TextAsDefaultLanguage() => localizeElement.Elements().FirstOrDefault(e => e.Name.LocalName[..2] == incaLocReader.DefaultCulture.Name[..2])?.Value;
            string? TextAsAnyLanguage() => localizeElement.Elements().FirstOrDefault()?.Value;
        }

        internal static XDocument? GetEmbeddedIncaLocFile(this Assembly assembly, IncaLocParameters incaLocParameters)
        {
            try
            {
                var fileName = incaLocParameters.FileName();
                var resourceName = assembly.FindManifestResourceName(fileName);
                using var stream = assembly.GetManifestResourceStream(resourceName);

                var xml = XDocument.Load(stream);

                return xml;
            }
            catch
            {
                return null;
            }
        }

        internal static IncaLocParameters GetParameters(this object toTranslate, string propertyName)
            => GetParameters(toTranslate.GetType(), propertyName);

        internal static IncaLocParameters GetParameters(this Type type, string propertyName)
            => GetParameters(type.Namespace, type.Name, propertyName);

        internal static IncaLocParameters GetParameters(string nameSpace, string className, string propertyName)
            => new IncaLocParameters(nameSpace, className, propertyName);
    }

}
