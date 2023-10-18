using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Tool
{
    internal class ViewModel
    {
        [IncaLocalize] public string TestProperty { get; }
    }
}

namespace Example.Tool.OtherNameSpace
{
    internal class ViewModel
    {
        [IncaLocalize] public string TestProperty { get; }
    }
}