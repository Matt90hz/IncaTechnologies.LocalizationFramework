using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
        public string? Title => GetText();

        /// <summary>
        /// Example of multi line string to translate.
        /// </summary>
        [IncaLocalize]
        public string? Description => GetText();

        /// <summary>
        /// Item source used to change the culture.
        /// </summary>
        public IEnumerable<string> SupportedCulture { get; } = new[] { "en-EN", "fr-FR", "es-ES" };

        /// <summary>
        /// Command to test the change of the culture.
        /// </summary>
        public ChangeCultureCommand ChangeCultureCommand { get; } = new ChangeCultureCommand();

        public ViewModelMainWindow()
        {
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
            IncaLocService.IncaLocReader.CurrentCulture = new CultureInfo((string?)parameter ?? "en-EN");
            CultureChanged?.Invoke(this, IncaLocService.IncaLocReader.CurrentCulture);
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
            return IncaLocService.IncaLocReader.GetText(new IncaLocParameters(
                nameSpace: this.GetType().Namespace,
                classIdentifier: this.GetType().Name,
                propertyIdentifier: propertyName));
        }
    }

}
