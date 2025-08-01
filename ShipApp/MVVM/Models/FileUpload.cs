using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShipApp.MVVM.Models
{
    public class FileUpload : INotifyPropertyChanged
    {
        public long Id { get; set; }
        public string? FileName { get; set; }
        public string? FileDriveId { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime TimeUploaded { get; set; }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
                }
            }
        }

        public override string ToString()
        {
            return $"FileName: {FileName}, DriveId: {FileDriveId}, IsProcessed: {IsProcessed}, TimeUploaded: {TimeUploaded}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
