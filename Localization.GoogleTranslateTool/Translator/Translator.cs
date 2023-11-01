using Google.Cloud.Translation.V2;
using Localization.GoogleTranslateTool.IncaLocaDocument;
using Localization.GoogleTranslateTool.Logger;
using Localization.GoogleTranslateTool.Translator._Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Localization.GoogleTranslateTool.Translator;

internal class Translator : ITranslator
{
    private readonly ILogger<Translator> _logger;

    public static ITranslator Default { get; } = new Translator(TranslatorLogger.Default);

    public Translator(ILogger<Translator> logger)
    {
        _logger = logger;
    }

    public async Task TranslateAsync(Uri incaLocFile, CultureInfo transalteFrom, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Translating from {culture}, {file}", transalteFrom.Name, incaLocFile.LocalPath);

        var incaLocDocument = await IncaLocDocument(incaLocFile, cancellationToken).ConfigureAwait(false);

        var incaLocDocuemntTransalated = await TranslateIncaDocumentAsync(incaLocDocument, transalteFrom, cancellationToken);

        var xml = incaLocDocuemntTransalated.ToXDocument();

        xml.Save(incaLocFile.LocalPath);
    }

    async Task<IncaLocDocument> TranslateIncaDocumentAsync(IncaLocDocument incaLocDocument, CultureInfo transalteFrom, CancellationToken cancellationToken = default)
    {
        var translateProperties = incaLocDocument.Properties.Select(property => TranslatePropertyAsync(property, transalteFrom, cancellationToken));

        var translatedProperties = await Task.WhenAll(translateProperties).ConfigureAwait(false);

        var transalatedIncaLocDocuemnt = incaLocDocument with
        {
            Properties = translatedProperties
        };

        return transalatedIncaLocDocuemnt;
    }

    async Task<Property> TranslatePropertyAsync(Property property, CultureInfo translateFrom, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Translating {property}.", property.Name);

        var fromTranslation = property.FindTranslation(translateFrom)
            ?? throw new($"The [{translateFrom.Name}] translation is not found. Cannot translate [{property.Name}].");

        var translateTranslations = property.Translations.Select(translation => TranslateTranslationAsync(translation, fromTranslation, cancellationToken));

        var transaltedTranslations = await Task.WhenAll(translateTranslations).ConfigureAwait(false);

        var translatedProperty = property with { Translations = transaltedTranslations };

        return translatedProperty;
    }

    async Task<Translation> TranslateTranslationAsync(Translation translation, Translation transalteFrom, CancellationToken cancellationToken = default)
    {
        if (translation.IsTranslated()) return translation;

        var translationClient = await TranslationClient.CreateAsync();

        var (text, to, from) = (transalteFrom.Text, translation.CultureInfo.Name, transalteFrom.CultureInfo.Name);

        var translationResult = await translationClient.TranslateTextAsync(text, to, from, TranslationModel.Base, cancellationToken).ConfigureAwait(false);

        var textTranslated = translationResult.TranslatedText;

        _logger.LogInformation("Translateted to [{culture}].", translation.CultureInfo.Name);

        var translatedTranslation = translation with { Text = textTranslated };

        return translatedTranslation;
    }

    static async Task<IncaLocDocument> IncaLocDocument(Uri incaLocFile, CancellationToken cancellationToken)
    {
        var isFilePath = incaLocFile.IsFile;
        var isIncaLocFile = incaLocFile.LocalPath.EndsWith(".incaloc");

        if (!isFilePath || !isIncaLocFile) throw new($"The file path {incaLocFile.LocalPath} do not point to a .incaloc file.");

        var incaLocText = await File.ReadAllTextAsync(incaLocFile.LocalPath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);

        var xDocument = XDocument.Parse(incaLocText);

        var incaLocDocument = xDocument.ToIncaLocDocument();

        return incaLocDocument;
    }

}
