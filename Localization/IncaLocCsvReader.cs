using Localization.Interfaces;
using System;
using System.Globalization;

namespace Localization
{
    /// <summary>
    /// Object to read .incaloc.csv files
    /// </summary>
    public class IncaLocCsvReader : IncaLocBase, IIncaLocReader
    {
        /// <summary>
        /// Default instance of <see cref="IncaLocCsvReader"/>.
        /// </summary>
        public static IncaLocReader Default { get; } = new IncaLocReader();

        /// <summary>
        /// Culture that will be extracted from the .incaloc.csv files by <see cref="GetText(IncaLocParameters)"/>.
        /// </summary>
        public virtual CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Culture that will be extracted form the .incaloc.csv files in case the current culture is not found.
        /// </summary>
        /// <remarks>
        /// Be sure that the culture setted in here is always present in the .incaloc.csv files.
        /// </remarks>
        public virtual CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Retrive the localized text form the .incaloc.csv file specified by the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Returns <c>string.Empty</c> if the translation is not found in the .incaloc.csv file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Throws when there is no .incaloc.csv file in the embedded resources</exception>
        public virtual string GetText(IncaLocParameters parameters)
        {
            throw new NotImplementedException();
        }
    }

}
