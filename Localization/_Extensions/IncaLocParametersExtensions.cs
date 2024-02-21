namespace Localization.Extensions
{
    internal static class IncaLocParametersExtensions
    {
        internal static string FileName(this IncaLocParameters parameters, string ext = "incaloc") => $"{parameters.NameSpace}.{parameters.ClassIdentifier}.{ext}";
    }
}
