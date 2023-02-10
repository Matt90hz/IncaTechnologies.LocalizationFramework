using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.Tools
{
    /// <summary>
    /// Helper class retrive executable references
    /// </summary>
    internal static class PortableExecutableReferenceProvider
    {
        /// <summary>
        /// Get a collection of <see cref="PortableExecutableReference"/> needed to interperet the semantic model for <see cref="Localization"/> classes.
        /// </summary>
        public static IEnumerable<PortableExecutableReference> PortableExecutableReferences { get; } = GetPortableExecutableReferences();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Get a collection of <see cref="PortableExecutableReference"/> needed to interperet the semantic model for <see cref="Localization"/> classes. </returns>
        /// <exception cref="Exception">Failed to retrive TRUSTED_PLATFORM_ASSEMBLIES</exception>
        static IEnumerable<PortableExecutableReference> GetPortableExecutableReferences()
        {
            //Get the reference for the referenced projects
            var result = new List<PortableExecutableReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IncaLocGenerator).Assembly.Location),
            };

            //Get the reference to the runtime enviroment
            var neededAssemblies = new[] { "System.Runtime", "netstandard" };

            var dotnetReferences = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?.Split(Path.PathSeparator)
                .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToArray() ?? throw new Exception("Failed to retrive TRUSTED_PLATFORM_ASSEMBLIES.");

            result.AddRange(dotnetReferences);

            return result;
        }
    }
}
