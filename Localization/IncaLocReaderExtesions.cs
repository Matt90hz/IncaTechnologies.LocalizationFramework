﻿using Localization.ExceptionResult;
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
        /// Get the localized text from the .incaloc file using the default <see cref="IIncaLocReader"/>. It uses the name of the caller as identifier.
        /// </summary>
        /// <param name="toTranslate">Used just to extract the type</param>
        /// <param name="propertyName"></param>
        /// <param name="incaLocReader"></param>
        /// <returns>Translated text or an empty string if nothing as been found.</returns>
        public static string GetText(this object toTranslate, [CallerMemberName] string propertyName = "", IIncaLocReader? incaLocReader = null)
        {
            var parameters = toTranslate.GetParameters(propertyName);
            var assembly = Assembly.GetAssembly(toTranslate.GetType());

            return (incaLocReader ?? IncaLocReader.Default).GetText(parameters, assembly);
        }

        internal static string GetText(this IIncaLocReader incaLocReader, IncaLocParameters parameters, Assembly assembly)
            => incaLocReader.GetText(parameters, assembly.GetEmbeddedIncaLocFile(parameters));

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

        internal static XDocument GetEmbeddedIncaLocFile(this Assembly assembly, IncaLocParameters incaLocParameters)
        {
            try
            {
                var fileName = incaLocParameters.FileName();
                var resourceName = assembly.FindManifestResourceName(fileName);
                var stream = assembly.GetManifestResourceStream(resourceName);

                var xml = XDocument.Load(stream);

                //Manually dispose of the stream just in case
                stream.Dispose();

                return xml;
            }
            catch (Exception e)
            {
                return new Exception<XDocument>("Failed to retrive XDocument from the embedded files.", e);
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
