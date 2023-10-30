using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.GoogleTranslateTool.Translator._Interfaces
{
    internal interface ITranslator
    {
        Task TranslateAsync(Uri incaLocFile, CultureInfo transalteFrom, CultureInfo[] translateTo, CancellationToken cancellationToken = default);
    }
}
