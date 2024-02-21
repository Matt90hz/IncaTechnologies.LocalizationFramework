using System;

namespace Localization
{
    /// <summary>
    /// Utility class that expose a singleton <see cref="IncaLocReader"/>.
    /// </summary>
    [Obsolete("Use IncaLocReader.Default instead.")]
    public static class IncaLocService
    {
        /// <summary>
        /// Get <see cref="Localization.IncaLocReader"/>.
        /// </summary>
        [Obsolete("Use IncaLocReader.Default instead.")]
        public static IncaLocReader IncaLocReader { get; set; } = new IncaLocReader();

    }
}
