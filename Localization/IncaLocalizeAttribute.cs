using System;

namespace Localization
{
    /// <summary>
    /// Attribute to mark a property. The property marked will be used by <see cref="IncaLocGenerator"/> to create .incaloc files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IncaLocalizeAttribute : Attribute
    {
        /// <summary>
        /// Create a new instance of <see cref="IncaLocalizeAttribute"/>
        /// </summary>
        public IncaLocalizeAttribute() { }
    }
}
