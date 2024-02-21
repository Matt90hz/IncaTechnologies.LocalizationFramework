using System.Globalization;

namespace Localization.Interfaces
{
    /// <summary>
    /// Reads .incaloc files.
    /// </summary>
    public interface IIncaLocReader
    {
        /// <summary>
        /// Culture that will be extracted from the .incaloc files by <see cref="GetText(IncaLocParameters)"/>.
        /// </summary>
        CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Culture that will be extracted form the .incaloc files in case the current culture is not found.
        /// </summary>
        /// <remarks>
        /// Be sure that the culture setted in here is always present in the .incaloc files.
        /// </remarks>
        CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Retrive the localized text form the .incaloc file specified by the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Returns <c>string.Empty</c> if the translation is not found in the .incaloc file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Throws when there is no .incaloc file in the embedded resources</exception>
        string GetText(IncaLocParameters parameters);
    }

}
