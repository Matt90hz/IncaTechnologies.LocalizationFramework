using Localization.ExceptionResult;
using System;
using System.IO;
using System.Text;

namespace Localization.Extensions
{
    internal static class StringBuilderExtensions
    {
        internal static Exception<bool> Save(this StringBuilder stringBuilder, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, stringBuilder.ToString());
                return true;
            }
            catch (Exception e)
            {
                return e.ToException<bool>();
            }
        }
    }

}