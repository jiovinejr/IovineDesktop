using ShipApp.Core;
using ShipApp.MVVM.Models;
using ShipApp.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShipApp.MVVM.ViewModels
{
    public class AddMeasurementViewModel : BaseViewModel
    {
        private readonly MeasurementService _measurementService = new();
        private readonly TaskCompletionSource<Measurement> _completionSource;
        private readonly RelayCommand _saveMeasurementCommand;

        private string _originalMeasurementName;
        public string OriginalMeasurementName
        {
            get => _originalMeasurementName;
            set => SetProperty(ref _originalMeasurementName, value);
        }

        private string _cleanMeasurementName;
        public string CleanMeasurementName
        {
            get => _cleanMeasurementName;
            set
            {
                SetProperty(ref _cleanMeasurementName, value);
                _saveMeasurementCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveMeasurementCommand => _saveMeasurementCommand;

        public AddMeasurementViewModel(string originalName, TaskCompletionSource<Measurement> completionSource)
        {
            _completionSource = completionSource;
            OriginalMeasurementName = originalName;

            _saveMeasurementCommand = new RelayCommand(async () => await SaveMeasurement(), CanSave);
        }

        private async Task SaveMeasurement()
        {
            try
            {
                var newMeasurement = new Measurement
                {
                    OriginalMeasurementName = this.OriginalMeasurementName,
                    CleanMeasurementName = this.CleanMeasurementName
                };

                var inserted = _measurementService.InsertNewMeasurement(newMeasurement);
                _completionSource.SetResult(inserted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to add measurement: {ex.Message}");
                _completionSource.SetException(ex);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(CleanMeasurementName);
        }
    }

}
