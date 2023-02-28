using Localization.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        public IncaLocGenerator(string storeLocation, string projectFile, IEnumerable<CultureInfo> cultureInfos = null) 
        { 
            StoreLocation = storeLocation;
            ProjectFile = projectFile;

            Cultures = cultureInfos ?? new List<CultureInfo>() { Thread.CurrentThread.CurrentCulture }; 
        }

        /// <inheritdoc/>
        public virtual void Generate(params IncaLocParameters[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (File.Exists(Path.Combine(StoreLocation, GetFileName(parameter))))
                {
                    Update(parameter);
                }
                else
                {
                    Generate(parameter);
                }
            }
        }

        /// <summary>
        /// Generates a new .incaloc file.
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="Exception"></exception>
        protected virtual void Generate(IncaLocParameters parameters)
        {
            if (!Cultures.Any()) throw new Exception($"{nameof(Cultures)} collection is empty!");

            var xml = new XDocument(
                new XElement(BASE_ELEMENT,
                new XAttribute(NAME_SPACE_ATTRIBUTE, parameters.NameSpace),
                new XAttribute(CLASS_ATTRIBUTE, parameters.ClassIdentifier),
                    CreateLocalizeElement(parameters.PropertyIdentifier)));

            var fileName = GetFileName(parameters);

            Save(xml, fileName);

            EmbedIncaLocFile(fileName);
            
        }

        /// <summary>
        /// Updates an existing .incaloc file.
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="FileNotFoundException"></exception>
        protected virtual void Update(IncaLocParameters parameters)
        {
            var incalocFile = (Path.Combine(StoreLocation, GetFileName(parameters)));

            if (!File.Exists(incalocFile)) throw new FileNotFoundException("Impossible to update the file: " + incalocFile + ". File not found.");

            var xml = XDocument.Load(incalocFile);

            if (xml.Element(BASE_ELEMENT).Elements(LOCALIZE_ELEMENT).Any(e => e.Attribute(PROPERTY_ATTRIBUTE).Value == parameters.PropertyIdentifier)) return;

            xml.Element(BASE_ELEMENT).Add(CreateLocalizeElement(parameters.PropertyIdentifier));

            Save(xml, GetFileName(parameters));
        }

        /// <summary>
        /// Creates a Localize element to be added to the XDocument.
        /// </summary>
        /// <param name="propertyAttribute"></param>
        /// <returns>
        /// <see cref="XElement"/> named Localize with an <see cref="XAttribute"/> named Property with a value of <paramref name="propertyAttribute"/>.
        /// Inside of it <see cref="XElement"/>s that rappresents all the <see cref="CultureInfo"/> in <see cref="Cultures"/>.
        /// </returns>
        private XElement CreateLocalizeElement(string propertyAttribute)
        {
            var result = new XElement(LOCALIZE_ELEMENT, new XAttribute(PROPERTY_ATTRIBUTE, propertyAttribute));

            result.Add(Cultures.Select(c => new XElement(c.Name, string.Empty)));

            return result;
        }

        /// <summary>
        /// Save the .incaloc <paramref name="document"/> with the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fileName"></param>
        private void Save(XDocument document, string fileName)
        {
            Directory.CreateDirectory(StoreLocation);

            document.Save(Path.Combine(StoreLocation, fileName));

        }

        /// <summary>
        /// Embbed as resounrces the specified <paramref name="fileName"/> in the <see cref="ProjectFile"/>.
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="FileNotFoundException"></exception>
        private void EmbedIncaLocFile(string fileName)
        {
            //Check if the MSBuild project file of reference exsists
            if (!File.Exists(ProjectFile)) throw new FileNotFoundException("Cannot find the specified .csproj file!");

            //Load the MSBuild project file as an Xml object
            var csproj = XElement.Load(ProjectFile, LoadOptions.PreserveWhitespace);

            //Try to retrive the ItemGroup labelled as "IncaLocalizeResources"
            var itemGroup = csproj.Elements().FirstOrDefault(e => e.Attribute("Label")?.Value == "IncaLocalizeResources" && e.Name == "ItemGroup");

            //If the ItemGroup is not there creates it
            if (itemGroup is null)
            {
                itemGroup = new XElement("ItemGroup", new XAttribute("Label", "IncaLocalizeResources"), new XText("\n\t"));

                csproj.Add(new XText("\n\t"));
                csproj.Add(itemGroup);
                csproj.Add(new XText("\n"));
            }

            //Generate file path
            var filePath = Path.Combine(StoreLocation, fileName);

            //It the resource is alredy been registered then exit
            if (itemGroup.Elements().Any(e => e.Name == "EmbeddedResource" && e.Attribute("Include")?.Value == filePath)) return;

            //Add the resource to the
            itemGroup.Add(new XText("\t"));
            itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", filePath)));
            itemGroup.Add(new XText("\n\t"));

            //Save the file
            File.WriteAllText(ProjectFile, csproj.ToString());

        }
    }

}