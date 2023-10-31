
using Localization.GoogleTranslateTool.Translator;


var fileToTranslate = new Uri(@"D:\VisualStudio\LocalizationFramework\Example\Example.ViewModelMainWindow.incaloc");
var translateFrom = new System.Globalization.CultureInfo("en-US");

var maybeException = await Translator.Default.TranslateAsync(fileToTranslate, translateFrom)
    .ContinueWith(task => task.Exception);

Console.WriteLine(maybeException);