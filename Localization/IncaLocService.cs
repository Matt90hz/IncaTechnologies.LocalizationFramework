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
    public static class IncaLocService
    {
        /// <summary>
        /// Get <see cref="Localization.IncaLocReader"/>.
        /// </summary>
        public static IncaLocReader IncaLocReader { get; set; } = new IncaLocReader();

    }
}
