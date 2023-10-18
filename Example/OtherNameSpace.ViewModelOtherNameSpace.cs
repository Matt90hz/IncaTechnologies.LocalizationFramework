using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Localization;

namespace Example.OtherNameSpace;

/// <summary>
/// Simple view model test namespace parsing.
/// </summary>
public class ViewModelOtherNameSpace : _ViewModelBase
{
    /// <summary>
    /// Asses what does get text on a localized property in other name spaces.
    /// </summary>
    [IncaLocalize] public string DecoratedProperty => GetText();

    /// <summary>
    /// Asses what does get text on a non localized property.
    /// </summary>
    public string NotDecoratedProperty => GetText();


}
