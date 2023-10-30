
using Localization.GoogleTranslateTool.Translator;


var fileToTranslate = new Uri(@"D:\VisualStudio\LocalizationFramework\Example\Example.ViewModelMainWindow.incaloc");
var translateFrom = new System.Globalization.CultureInfo("en-US");
var translateTo = new System.Globalization.CultureInfo[]
{
    new System.Globalization.CultureInfo("fr-FR"),
    new System.Globalization.CultureInfo("es-ES"),
};

var log = await Translator.Default.TranslateAsync(fileToTranslate, translateFrom, translateTo)
    .ContinueWith(task => task.IsCompletedSuccessfully ? "Stuff should be translated" : task.Exception?.InnerException?.ToString());

Console.WriteLine(log);