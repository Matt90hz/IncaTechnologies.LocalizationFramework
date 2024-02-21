using Localization.Interfaces;
using System.Globalization;
using System.Reflection;

namespace Localization
{
    /// <summary>
    /// Object to read .incaloc files
    /// </summary>
    public class IncaLocReader : IncaLocBase, IIncaLocReader
    {
        /// <summary>
        /// Default instance of <see cref="IncaLocReader"/>.
        /// </summary>
        public static IncaLocReader Default { get; } = new IncaLocReader();

        /// <inheritdoc/>
        public virtual CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <inheritdoc/>
        public virtual CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <inheritdoc/>
        public virtual string GetText(IncaLocParameters parameters) => this.GetText(parameters, Assembly.GetCallingAssembly());

    }

}
