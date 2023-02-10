using System.Collections.Generic;
using System.Globalization;

namespace Localization.Interfaces
{
    /// <summary>
    /// Generates .incalod files.
    /// </summary>
    public interface IIncaLocGenerator 
    {
        /// <summary>
        /// All the <see cref="CultureInfo"/> that will be added to the .incaloc files generated.
        /// </summary>
        IEnumerable<CultureInfo> Cultures { get; }

        /// <summary>
        /// The path of the folder in which the .incaloc files will be saved. 
        /// </summary>
        string StoreLocation { get; }

        /// <summary>
        /// The path of the .csproj file in which the .incaloc files will be added as embedded resources.
        /// </summary>
        string ProjectFile { get; }

        /// <summary>
        /// Generate or update the .incaloc file specified by the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        void Generate(params IncaLocParameters[] parameters);
    }

}
