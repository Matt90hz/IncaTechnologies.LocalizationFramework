

namespace Localization
{
    /// <summary>
    /// Parameters used to generate .incaloc files.
    /// </summary>
    public readonly struct IncaLocParameters 
    {
        /// <summary>
        /// Namespace of the class.
        /// </summary>
        public string NameSpace { get; }

        /// <summary>
        /// Class identifier.
        /// </summary>
        public string ClassIdentifier { get; }

        /// <summary>
        /// Property identifier.
        /// </summary>
        public string PropertyIdentifier { get; }

        /// <summary>
        /// Create a new instance of <see cref="IncaLocParameters"/>.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="classIdentifier"></param>
        /// <param name="propertyIdentifier"></param>
        public IncaLocParameters(string nameSpace, string classIdentifier, string propertyIdentifier)
        {
            NameSpace = nameSpace;
            ClassIdentifier = classIdentifier;
            PropertyIdentifier = propertyIdentifier;
        }

        /// <inheritdoc/>
        public override string ToString() => string.Format("{0}.{1}.{2}", NameSpace, ClassIdentifier, PropertyIdentifier);

    }
}
