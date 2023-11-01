using CommandLine;
using Localization.GoogleTranslateTool._Extensions;
using Localization.GoogleTranslateTool.Logger;
using Localization.GoogleTranslateTool.Translator;
using Microsoft.Extensions.Logging;
using System.Globalization;

//parse arguments
var arguments = Parser.Default.ParseArguments<Arguments>(args).Value;

//if argument fail to parse a message from CommandLineParser will be displayed
if (arguments is null) return;

//if the verbose option is on , the logger will log all messages, otherwise only warnings and errors
Func<LogLevel, bool> loggerFilter = arguments.Verbose
    ? (logLevel => logLevel >= LogLevel.Information)
    : (logLevel => logLevel >= LogLevel.Warning);

//create logger and translator
var logger = new TranslatorLogger(loggerFilter);
var translator = new Translator(logger);

//translate
try
{
    //create uri from the input path
    var inputUri = new Uri(arguments.IncaLocFilePath);

    //if the input path is a directory, get all .incaloc files in the directory and subdirectories
    var incaLocFiles = inputUri.IsDirectory()
        ? Directory.GetFiles(inputUri.LocalPath, "*.incaloc", SearchOption.AllDirectories).Select(x => new Uri(x))
        : new[] { inputUri };

    //if no .incaloc files are found, log a warning and exit
    if (!incaLocFiles.Any())
    {
        logger.LogWarning("No .incaloc files found in {directory}", inputUri.LocalPath);
        return;
    }

    //create culture info from the translate from argument
    var cultureInfoFrom = new CultureInfo(arguments.TranslateFrom);

    //create the transaltion tasks and log errors in case of failure
    var translateTasks = incaLocFiles.Select(file => translator.TranslateAsync(file, cultureInfoFrom));

    //if the verbose option is on, await each task individually, otherwise await all tasks
    if (arguments.Verbose)
    {
        foreach (var task in translateTasks) await task.ConfigureAwait(false);
    }
    else
    {
        await Task.WhenAll(translateTasks).ConfigureAwait(false);
    }
}
catch (UriFormatException ex)
{
    logger.LogError(ex, "Invalid file path");
}
catch (CultureNotFoundException ex)
{
    logger.LogError(ex, "Invalid culture");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error while translating");
}
