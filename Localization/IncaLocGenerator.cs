using Localization.ExceptionResult;
using Localization.Extensions;
using Localization.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Localization
{
    /// <summary>
    /// Object to generate .incaloc files.
    /// </summary>
    public class IncaLocGenerator : IncaLocBase, IIncaLocGenerator
    {
        /// <inheritdoc/>
        public IEnumerable<CultureInfo> Cultures { get; }

        /// <inheritdoc/>
        public string StoreLocation { get; }

        /// <inheritdoc/>
        public string ProjectFile { get; }

        /// <summary>
        /// Create an instance of <see cref="IncaLocGenerator"/> and initialize the properties.
        /// </summary>
        /// <param name="storeLocation"></param>
        /// <param name="projectFile"></param>
        /// <param name="cultureInfos"></param>
        public IncaLocGenerator(string storeLocation, string projectFile, IEnumerable<CultureInfo>? cultureInfos = null)
        {
            StoreLocation = storeLocation;
            ProjectFile = projectFile;
            Cultures = cultureInfos ?? new[] { CultureInfo.CurrentCulture };
        }

        /// <summary>
        /// Create an instance of <see cref="IncaLocGenerator"/> and initialize the properties.
        /// </summary>
        /// <param name="storeLocation"></param>
        /// <param name="projectFile"></param>
        /// <param name="cultureInfos"></param>
        public IncaLocGenerator(Uri storeLocation, Uri projectFile, IEnumerable<CultureInfo>? cultureInfos = null) : this(storeLocation.LocalPath, projectFile.LocalPath, cultureInfos) { }

        /// <inheritdoc/>
        public virtual void Generate(params IncaLocParameters[] parameters)
        {
            foreach (var parameter in parameters)
            {
                var filePath = Path.Combine(StoreLocation, parameter.FileName());

                Directory.CreateDirectory(StoreLocation);

                if (File.Exists(filePath))
                {
                    LocalizeDocument(parameter, Cultures, XDocument.Load(filePath)).Save(filePath);
                }
                else
                {
                    LocalizeDocument(parameter, Cultures).Save(filePath);
                }

                IncaLocalizeResources.EmbedFile(ProjectFile, filePath).Value();
            }
        }

        static XDocument LocalizeDocument(IncaLocParameters parameters, IEnumerable<CultureInfo> cultures, XDocument document)
        {
            var localizeElement = document.DescendantWithAttribute(parameters.PropertyIdentifier);

            if (localizeElement is null)
            {
                document
                    .Element(BASE_ELEMENT)
                    .AddElement(LocalizeElement(parameters))
                    .Elements().Last()
                    .AddElements(CultureElements(cultures));
            }
            else
            {
                var missingCultures = cultures
                    .Where(culture => localizeElement.Element(culture.Name) is null)
                    .Select(culture => CultureElement(culture));

                localizeElement.AddElements(missingCultures);
            }

            return document;
        }

        static XDocument LocalizeDocument(IncaLocParameters parameters, IEnumerable<CultureInfo> cultures)
        {
            try
            {
                return new XDocument()
                    .AddElement(BaseElement(parameters))
                    .Elements().Last()
                    .AddElement(LocalizeElement(parameters))
                    .Elements().Last()
                    .AddElements(CultureElements(cultures))
                    .Document;
            }
            catch (Exception e)
            {
                return new Exception<XDocument>("Failed to create localize document", e);
            }
        }

        static XElement BaseElement(IncaLocParameters parameters) =>
            new XElement(BASE_ELEMENT, new XAttribute(NAME_SPACE_ATTRIBUTE, parameters.NameSpace), new XAttribute(CLASS_ATTRIBUTE, parameters.ClassIdentifier));

        static XElement LocalizeElement(IncaLocParameters parameters) =>
            new XElement(LOCALIZE_ELEMENT, new XAttribute(PROPERTY_ATTRIBUTE, parameters.PropertyIdentifier));

        static IEnumerable<XElement> CultureElements(IEnumerable<CultureInfo> cultureInfos) =>
            cultureInfos.Select(CultureElement);

        static XElement CultureElement(CultureInfo cultureInfo) =>
            new XElement(cultureInfo.Name, string.Empty);
    }

}