using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ShipApp.Core;
using ShipApp.MVVM.Models;
using ShipApp.Service;
using CommunityToolkit.Maui;

namespace ShipApp.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<FileUpload> FilesToProcess { get; }
        public ObservableCollection<Ship> ShipsList { get; }

        public ICommand ProcessCommand { get; }
        public ICommand ShowAddItemPopupCommand { get; }

        private readonly IPopupService _popupService;
        private readonly ItemService _itemService;
        private readonly ShipService _shipService;
        private readonly FileUploadService _fileUploadService;
        private readonly MeasurementService _measurementService;
        private readonly OrderRecordService _orderRecordService;

        // DI constructor
        public HomeViewModel(
            IPopupService popupService,
            ItemService itemService,
            ShipService shipService,
            FileUploadService fileUploadService,
            MeasurementService measurementService,
            OrderRecordService orderRecordService)
        {
            _popupService = popupService;
            _itemService = itemService;
            _shipService = shipService;
            _fileUploadService = fileUploadService;
            _measurementService = measurementService;
            _orderRecordService = orderRecordService;

            FilesToProcess = _fileUploadService.GetFilesToProcess();
            ShipsList = _shipService.GetShips();

            ProcessCommand = new RelayCommand<FileUpload>(
                async file => await ProcessFileAsync(file),
                file => file is not null && !file.IsProcessing);

            ShowAddItemPopupCommand = new RelayCommand(async () => await ShowAddItemPopup());
        }

        private async Task ShowAddItemPopup()
        {
            var args = new Dictionary<string, object>
            {
                [nameof(AddItemViewModel.OriginalItemName)] = "SAMPLE ORIGINAL"
            };

            // Fire it up; ignore the result for a quick visual check
            await _popupService.ShowPopupAsync<AddItemViewModel, ShipApp.MVVM.Models.Item?>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: args);
        }

        private async Task ProcessFileAsync(FileUpload file)
        {
            try
            {
                file.IsProcessing = true;

                var driveService = await GoogleAuthHelper.GetDriveServiceAsync();
                var downloader = new DriveDownloadService(driveService);

                using var stream = await downloader.DownloadFileAsync(file.FileDriveId);
                var rdf = new ReadDataFile(stream, file.FileName);
                var records = rdf.CreateExcelOrderList();

                string shipNameFromRecord = records[0].ShipName;
                var ship = _shipService.GetShipByShipName(shipNameFromRecord);
                if (ship != null) 
                {
                    _orderRecordService.DeleteOrderRecordsByShip(ship);
                }
                else
                {
                    ship = _shipService.AddNewShip(new Ship(shipNameFromRecord));
                }

                foreach (var record in records)
                {
                    var item = _itemService.GetItemByOriginalName(record.Item)
                               ?? await _itemService.HandleUnknownItemAsync(record.Item);

                    if (item is null) { Debug.WriteLine($"❌ Item still not found: {record.Item}"); continue; }

                    var measurement = _measurementService.GetMeasurementObjectByOriginalName(record.Measurement)
                                     ?? await _measurementService.HandleUnknownMeasurement(record.Measurement);

                    if (measurement is null) { Debug.WriteLine($"❌ Measurement still not found: {record.Measurement}"); continue; }

                    _orderRecordService.InsertNewOrderRecord(record, ship, item, measurement);
                }

                FilesToProcess.Remove(file);
                ShipsList.Add(ship);
                await downloader.RenameDriveFile(file.FileDriveId, shipNameFromRecord);
                _fileUploadService.MarkFileAsProcessed(file.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Download error: {ex.Message}");
            }
            finally
            {
                file.IsProcessing = false;
            }
        }
    }
}
