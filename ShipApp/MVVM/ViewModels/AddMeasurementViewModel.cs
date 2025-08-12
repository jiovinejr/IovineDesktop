using CommunityToolkit.Maui;          // IPopupService
using Microsoft.Maui.Controls;        // IQueryAttributable
using ShipApp.Core;
using ShipApp.MVVM.Models;
using System.Windows.Input;

namespace ShipApp.MVVM.ViewModels
{
    public class AddMeasurementViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IPopupService _popupService;
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        public string OriginalMeasurementName
        {
            get => _originalMeasurementName;
            set => SetProperty(ref _originalMeasurementName, value);
        }
        private string _originalMeasurementName = string.Empty;

        public string CleanMeasurementName
        {
            get => _cleanMeasurementName;
            set { SetProperty(ref _cleanMeasurementName, value); _saveCommand.RaiseCanExecuteChanged(); }
        }
        private string _cleanMeasurementName = string.Empty;

        public ICommand SaveMeasurementCommand => _saveCommand;
        public ICommand CancelCommand => _cancelCommand;

        public AddMeasurementViewModel(IPopupService popupService)
        {
            _popupService = popupService;
            _saveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            _cancelCommand = new RelayCommand(async () => await _popupService.ClosePopupAsync(Shell.Current));
        }

        // This is how Shell parameters set OriginalMeasurementName:
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(nameof(OriginalMeasurementName), out var v) && v is string s)
                OriginalMeasurementName = s;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(CleanMeasurementName);

        private async Task SaveAsync()
        {
            var measurement = new Measurement
            {
                OriginalMeasurementName = OriginalMeasurementName,
                CleanMeasurementName = CleanMeasurementName
            };

            // Close popup and return the new (unsaved) Measurement to the caller
            await _popupService.ClosePopupAsync(Shell.Current, measurement);
        }
    }
}
