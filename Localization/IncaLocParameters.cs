

namespace Localization
{
    public readonly struct IncaLocParameters 
    {
        public string NameSpace { get; }

        public string ClassIdentifier { get; }

        public string PropertyIdentifier { get; }

        public IncaLocParameters(string nameSpace, string classIdentifier, string propertyIdentifier)
        {
            NameSpace = nameSpace;
            ClassIdentifier = classIdentifier;
            PropertyIdentifier = propertyIdentifier;
        }

    }
}
