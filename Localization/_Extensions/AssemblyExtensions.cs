using System.Linq;
using System.Reflection;

namespace Localization.Extensions
{
    internal static class AssemblyExtensions 
    {
        internal static string? FindManifestResourceName(this Assembly assembly, string name) => assembly.GetManifestResourceNames().FirstOrDefault(res => res.Contains(name));
    }
}
