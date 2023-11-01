using System.Globalization;

namespace Localization.GoogleTranslateTool.Translator._Interfaces;

internal interface ITranslator
{
    Task TranslateAsync(Uri incaLocFile, CultureInfo transalteFrom, CancellationToken cancellationToken = default);
}
