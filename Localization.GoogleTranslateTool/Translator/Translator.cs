using Google.Cloud.Translation.V2;
using Localization.GoogleTranslateTool.IncaLocaDocument;
using Localization.GoogleTranslateTool.IncaLocaDocument._Extensions;
using Localization.GoogleTranslateTool.Translator._Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Localization.GoogleTranslateTool.Translator
{
    internal class Translator : ITranslator
    {
        public static ITranslator Default { get; } = new Translator();

        public async Task TranslateAsync(Uri incaLocFile, CultureInfo transalteFrom, CancellationToken cancellationToken = default)
        {
            var incaLocDocument = await IncaLocDocument(incaLocFile, cancellationToken).ConfigureAwait(false);

            var incaLocDocuemntTransalated = await TranslateIncaDocumentAsync(incaLocDocument, transalteFrom, cancellationToken);

            var xml = incaLocDocuemntTransalated.ToXDocument();
                
            xml.Save(incaLocFile.LocalPath);
        }

        static async Task<IncaLocDocument> TranslateIncaDocumentAsync(IncaLocDocument incaLocDocument, CultureInfo transalteFrom, CancellationToken cancellationToken = default)
        {
            var translateProperties = incaLocDocument.Properties.Select(property => TranslatePropertyAsync(property, transalteFrom, cancellationToken));

            var translatedProperties = await Task.WhenAll(translateProperties).ConfigureAwait(false);

            var transalatedIncaLocDocuemnt = incaLocDocument with
            {
                Properties = translatedProperties
            };

            return transalatedIncaLocDocuemnt;
        }

        static async Task<Property> TranslatePropertyAsync(Property property, CultureInfo translateFrom, CancellationToken cancellationToken = default)
        {
            var fromTranslation = property.FindTranslation(translateFrom) 
                ?? throw new($"{translateFrom.Name} translation not found. Cannot translate {property.Name} from {translateFrom.Name} culture.");

            var translateTranslations = property.Translations.Select(translation => TranslateTranslationAsync(translation, fromTranslation, cancellationToken));

            var transaltedTranslations = await Task.WhenAll(translateTranslations).ConfigureAwait(false);

            var translatedProperty = property with { Translations = transaltedTranslations };

            return translatedProperty;
        }

        static async Task<Translation> TranslateTranslationAsync(Translation translation, Translation transalteFrom, CancellationToken cancellationToken = default)
        {
            if (translation.IsTranslated()) return translation;

            var translationClient = await TranslationClient.CreateAsync();

            var (text, to, from) = (transalteFrom.Text, translation.CultureInfo.Name, transalteFrom.CultureInfo.Name);

            var translationResult = await translationClient.TranslateTextAsync(text, to, from, TranslationModel.Base, cancellationToken).ConfigureAwait(false);

            var textTranslated = translationResult.TranslatedText;

            var translatedTranslation = translation with { Text = textTranslated };

            return translatedTranslation;
        }

        static async Task<IncaLocDocument> IncaLocDocument(Uri incaLocFile, CancellationToken cancellationToken)
        {
            var isFilePath = incaLocFile.IsFile;
            var isIncaLocFile = incaLocFile.LocalPath.EndsWith(".incaloc");   

            if (!isFilePath || !isIncaLocFile) throw new($"The file path {incaLocFile.LocalPath} do not point to a .incaloc file.");

            var incaLocText = await File.ReadAllTextAsync(incaLocFile.LocalPath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            var xDocuemnt = XDocument.Parse(incaLocText);

            var incaLocDocument = xDocuemnt.ToIncaLocDocument();

            return incaLocDocument;
        }

    }



}
