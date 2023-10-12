using Localization.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
