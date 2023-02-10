using System;
using System.Globalization;
using System.Text;

namespace Localization.Interfaces
{
    /// <summary>
    /// Reads .incaloc files.
    /// </summary>
    public interface IIncaLocReader 
    {
        /// <summary>
        /// Culture that will be extected from the .incaloc files by <see cref="GetText(IncaLocParameters)"/>.
        /// </summary>
        CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Culture that will be extracted form the .incaloc files in case the culture is not found.
        /// </summary>
        /// <remarks>
        /// Be sure that the culture setted in here is always present in the .incaloc files.
        /// </remarks>
        CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Retrive the localized text form the .incaloc file specified by the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Returns <c>null</c> if the translation is not found in the .incaloc file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        string GetText(IncaLocParameters parameters);
    }

}
