namespace Localization
{
    /// <summary>
    /// Base class for the elements in common between <see cref="IncaLocGenerator"/> and <see cref="IncaLocReader"/>.
    /// </summary>
    public abstract class IncaLocBase
    {
        /// <summary>
        /// The base element of the xml file.
        /// </summary>
        protected const string BASE_ELEMENT = "IncaTehcnologies";

        /// <summary>
        /// The name given to the namespace attribute.
        /// </summary>
        protected const string NAME_SPACE_ATTRIBUTE = "NameSpace";

        /// <summary>
        /// The name given to the class attribute.
        /// </summary>
        protected const string CLASS_ATTRIBUTE = "Class";

        /// <summary>
        /// The name given to the localize element.
        /// </summary>
        protected const string LOCALIZE_ELEMENT = "Localize";

        /// <summary>
        /// The name given to the property attribute.
        /// </summary>
        protected const string PROPERTY_ATTRIBUTE = "Property";
    }

}
