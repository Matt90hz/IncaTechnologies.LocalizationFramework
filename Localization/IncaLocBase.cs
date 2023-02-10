using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Localization
{
    /// <summary>
    /// Base class for the elements in common between <see cref="IncaLocGenerator"/> and <see cref="IncaLocReader"/>.
    /// </summary>
    public abstract class IncaLocBase
    {
        protected const string BASE_ELEMENT = "IncaTehcnologies";
        protected const string NAME_SPACE_ATTRIBUTE = "NameSpace";
        protected const string CLASS_ATTRIBUTE = "Class";
        protected const string LOCALIZE_ELEMENT = "Localize";
        protected const string PROPERTY_ATTRIBUTE = "Property";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>File name for the given <paramref name="parameters"/>.</returns>
        protected string GetFileName(IncaLocParameters parameters)
        {
            return $"{parameters.NameSpace}.{parameters.ClassIdentifier}.incaloc";
        }
    }

}
