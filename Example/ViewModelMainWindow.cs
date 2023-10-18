using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Example.OtherNameSpace;
using Localization;

namespace Example
{
    /// <summary>
    /// Simple view model to show how translation actually take place.
    /// </summary>
    public class ViewModelMainWindow : _ViewModelBase
    {
        /// <summary>
        /// Example of a string to translate.
        /// </summary>
        [IncaLocalize]
        public string Title => GetText();

        /// <summary>
        /// Example of multi line string to translate.
        /// </summary>
        [IncaLocalize]
        public string Description => GetText();

        /// <summary>
        /// Item source used to change the culture.
        /// </summary>
        public IEnumerable<string> SupportedCulture { get; } = new[] { "en-EN", "fr-FR", "es-ES", "en", "es", "fr", "en-US", "cz" };

        /// <summary>
        /// Command to test the change of the culture.
        /// </summary>
        public ChangeCultureCommand ChangeCultureCommand { get; } = new ChangeCultureCommand();

        /// <summary>
        /// Test the diffrent namespace localization.
        /// </summary>
        public ViewModelOtherNameSpace OtherNameSpace { get; } = new ViewModelOtherNameSpace();

        /// <summary>
        /// Create e new instance of <see cref="ViewModelMainWindow"/> and initializes it.
        /// </summary>
        public ViewModelMainWindow()
        {
            IncaLocReader.Default.CurrentCulture = new CultureInfo("en-EN");
            IncaLocReader.Default.DefaultCulture = new CultureInfo("en");

            ChangeCultureCommand.CultureChanged += ChangeCultureCommand_CultureChanged;
        }

        /// <summary>
        /// Update the view when the culture change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeCultureCommand_CultureChanged(object? sender, CultureInfo e)
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(OtherNameSpace));
        }
    }

    /// <summary>
    /// Changes the culture of <see cref="IncaLocReader"/>.
    /// </summary>
    public class ChangeCultureCommand : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Signal the culture has changed.
        /// </summary>
        public event EventHandler<CultureInfo>? CultureChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            IncaLocReader.Default.CurrentCulture = new CultureInfo((string?)parameter ?? "en-EN");
            CultureChanged?.Invoke(this, IncaLocReader.Default.CurrentCulture);
        }
    }

    /// <summary>
    /// Example of how to build a base class that infer the <see cref="IncaLocParameters"/>.
    /// </summary>
    public abstract class _ViewModelBase : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Standard property changed handling.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// <c>GetText()</c> that can be called parameterless directly from the properti to localize.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual string GetText([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return string.Empty;

            return IncaLocReader.Default.GetText(new IncaLocParameters(
                nameSpace: GetType().Namespace!,
                classIdentifier: GetType().Name,
                propertyIdentifier: propertyName));
        }
    }

}
