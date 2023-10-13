using Localization.ExceptionResult;
using Localization.Extensions;
using System;
using System.IO;
using System.Xml.Linq;

namespace Localization
{
    internal static class IncaLocalizeResources
    {
        const string INCLUDE = "Include";
        const string LABEL = "Label";
        const string ITEM_GROUP = "ItemGroup";
        const string LABEL_INCA_LOCALIZE_RESOURCES = "IncaLocalizeResources";
        const string ELEMENT_EMBEDDED_RESOURCE = "EmbeddedResource";

        internal static Exception<bool> EmbedFile(string projectPath, string embedPath)
        {
            try
            {
                var project = Project(projectPath).Value();

                if (project.HasDescendantWithAttribute(embedPath)) return true;

                if (project.HasDescendantWithAttribute(LABEL_INCA_LOCALIZE_RESOURCES) is false) project.AddElement(IncaLocalizeResourcesElement());

                project
                    .DescendantWithAttribute(LABEL_INCA_LOCALIZE_RESOURCES)!
                    .AddNode(new XText("\t"))
                    .AddElement(EmbeddedResourceElement(embedPath))
                    .AddNode(new XText("\n\t"));

                File.WriteAllText(projectPath, project.ToString());

                return true;
            }
            catch (Exception e)
            {
                return new Exception<bool>("Failed to embed files.", e);
            }
        }

        static XElement IncaLocalizeResourcesElement() => new XElement(ITEM_GROUP, new XAttribute(LABEL, LABEL_INCA_LOCALIZE_RESOURCES), new XText("\n\t"));

        static XElement EmbeddedResourceElement(string resourcePath) => new XElement(ELEMENT_EMBEDDED_RESOURCE, new XAttribute(INCLUDE, resourcePath));

        static Exception<Uri> ProjectUri(string projectPath)
        {
            try
            {
                var uri = new Uri(projectPath);

                return !uri.IsFileExtension("csproj") ? new Exception<Uri>("The specified file is not a .csproj file!") : uri.ToException();
            }
            catch (Exception e)
            {
                return new Exception<Uri>("Invalid ProjectPath", e);
            }
        }

        static Exception<XElement> Project(string projectFolder)
        {
            try
            {
                var projectUri = ProjectUri(projectFolder).Value();

                return XElement.Load(projectFolder, LoadOptions.PreserveWhitespace);
            }
            catch (Exception e)
            {
                return new Exception<XElement>("Failed to create the project.", e);
            }
        }
    }

}