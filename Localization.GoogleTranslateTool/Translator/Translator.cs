using Google.Cloud.Translation.V2;
using Localization.GoogleTranslateTool.Translator._Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Localization.GoogleTranslateTool.Translator
{
    internal class Translator : ITranslator
    {
        public static ITranslator Default { get; } = new Translator();

        public async Task TranslateAsync(Uri incaLocFile, CultureInfo transalteFrom, CultureInfo[] translateTo, CancellationToken cancellationToken = default)
        {
            var incaLocDocument = await IncaLocText(incaLocFile, cancellationToken).ConfigureAwait(false);          

            await UpdateIncaLocDocumentAsync(incaLocDocument, transalteFrom, translateTo, cancellationToken);

            using var fileStream = File.OpenWrite(incaLocFile.LocalPath);

            incaLocDocument.Save(fileStream);
        }

        private async static Task UpdateIncaLocDocumentAsync(XDocument incaLocDocument, CultureInfo transalteFrom, CultureInfo[] translateTo, CancellationToken cancellationToken = default)
        {
            var textToTranslate = TextToTranslateAsync(incaLocDocument, transalteFrom);

            var translatedText = await TranslateAsync(textToTranslate, transalteFrom, translateTo, cancellationToken).ConfigureAwait(false);

            foreach(var (property, cultureInfo, text) in translatedText)
            {
                var localizeElement = incaLocDocument
                    .Descendants(IncaLocTags.LOCALIZE_ELEMENT)
                    .First(x => x.Attribute(IncaLocTags.PROPERTY_ATTRIBUTE)?.Value == property);

                var cultureElement = localizeElement.Element(cultureInfo.Name);

                if (cultureElement is null) continue;

                cultureElement.Value = text;
            }
        }

        private static async Task<IEnumerable<(string Property, CultureInfo CultureInfo, string Value)>> TranslateAsync(IEnumerable<(string Property, string Text)> textToTranslate, CultureInfo transalteFrom, CultureInfo[] translateTo, CancellationToken cancellationToken = default)
        {
            var translateTextTasks = translateTo.Select(x => TranslateAsync(textToTranslate, transalteFrom, x, cancellationToken));

            var completedTransaltions = await Task.WhenAll(translateTextTasks).ConfigureAwait(false);

            var flattenedTranslations = completedTransaltions.SelectMany(x => x);

            return flattenedTranslations;
        }

        private static async Task<IEnumerable<(string Property, CultureInfo CultureInfo, string Value)>> TranslateAsync(IEnumerable<(string Property, string Text)> textToTranslate, CultureInfo transalteFrom, CultureInfo translateTo, CancellationToken cancellationToken = default)
        {
            var transalteTextTasks = textToTranslate.Select(x => TranslateAsync(x, transalteFrom, translateTo, cancellationToken));

            var completedTranslations = await Task.WhenAll(transalteTextTasks).ConfigureAwait(false);

            return completedTranslations;
        }

        private static async Task<(string Property, CultureInfo CultureInfo, string Value)> TranslateAsync((string Property, string Text) textToTranslate, CultureInfo transalteFrom, CultureInfo translateTo, CancellationToken cancellationToken = default)
        {
            var (property, text) = textToTranslate;

            var translationClinet = await TranslationClient.CreateAsync();

            var translationResult = await translationClinet.TranslateTextAsync(text, translateTo.Name, transalteFrom.Name, TranslationModel.Base, cancellationToken).ConfigureAwait(false);

            var textTranslated = translationResult.TranslatedText;

            return (property, translateTo, textTranslated);
        }

        private static IEnumerable<(string Property, string Text)> TextToTranslateAsync(XDocument incaLocDocument, CultureInfo transalteFrom)
        {
            var localizeElements = incaLocDocument.Descendants(IncaLocTags.LOCALIZE_ELEMENT);

            var textToTranslate = localizeElements.Select(localizeElement =>
            {
                var property = localizeElement.Attribute(IncaLocTags.PROPERTY_ATTRIBUTE)?.Value
                    ?? throw new($"Localize element do not have a property attribute.");

                var text = localizeElement.Element(transalteFrom.Name)?.Value
                    ?? throw new($"The culture to translate from is not found.");

                return (property, text);
            });

            return textToTranslate;
        }

        private static async Task<XDocument> IncaLocText(Uri incaLocFile, CancellationToken cancellationToken)
        {
            if (IsInvalid(incaLocFile)) throw new($"The file path {incaLocFile.LocalPath} do not point to a .incaloc file.");

            var incaLocText = await File.ReadAllTextAsync(incaLocFile.LocalPath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            var incaLocDocuemnt = XDocument.Parse(incaLocText);

            return incaLocDocuemnt;
        }

        private static bool IsInvalid(Uri incaLocFile)
        {
            var isFilePath = incaLocFile.IsFile;
            var isIncaLocFile = incaLocFile.LocalPath.EndsWith(".incaloc");

            return !isFilePath || !isIncaLocFile;
        }
    }
}
