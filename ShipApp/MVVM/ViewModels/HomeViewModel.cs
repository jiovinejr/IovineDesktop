using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipApp.Core;
using ShipApp.Service;
using ShipApp.MVVM.Models;
using System.Windows.Input;
using System.Diagnostics;
using ShipApp.MVVM.Views;

namespace ShipApp.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<FileUpload> FilesToProcess { get; set; }
        public ObservableCollection<Ship> ShipsList { get; set; }
        public ICommand ProcessCommand { get; }

        private readonly ItemService _itemService;
        private readonly ShipService _shipService;
        private readonly FileUploadService _fileUploadService;
        private readonly MeasurementService _measurementService;
        private readonly OrderRecordService _orderRecordService;

        public HomeViewModel()
        {
            _fileUploadService = new FileUploadService();
            _measurementService = new MeasurementService();
            _shipService = new ShipService();
            _itemService = new ItemService();
            _orderRecordService = new OrderRecordService();


            FilesToProcess = _fileUploadService.GetFilesToProcess();
            ShipsList = _shipService.GetShips();

            ProcessCommand = new RelayCommand<FileUpload>(
                async (file) => await ProcessFileAsync(file),
                (file) => file != null && !file.IsProcessing
                );

        }


        private async Task ProcessFileAsync(FileUpload file)
        {
            try
            {
                file.IsProcessing = true;

                var driveService = await GoogleAuthHelper.GetDriveServiceAsync();
                var downloader = new DriveDownloadService(driveService);

                //Download Logic
                MemoryStream stream = await downloader.DownloadFileAsync(file.FileDriveId);
                var rdf = new ReadDataFile(stream, file.FileName);
                List<ExcelRecord> records = rdf.CreateExcelOrderList();

                //Add Ship
                string shipNameFromRecord = records[0].ShipName;
                Ship ship = _shipService.GetShipByShipName(shipNameFromRecord);
                if (ship == null)
                {
                    ship = _shipService.AddNewShip(new Ship(shipNameFromRecord));
                }
                else
                {
                    _orderRecordService.DeleteOrderRecordsByShip(ship);
                }

                //Loop Through excel file
                foreach (ExcelRecord record in records)
                {
                    Item item = _itemService.GetItemByOriginalName(record.Item);
                    Measurement measurement = _measurementService.GetMeasurementObjectByOriginalName(record.Measurement);
                    if (item == null)
                    {
                        item = await _itemService.HandleUnknownItemAsync(record.Item);
                        if (item == null)
                        {
                            Debug.WriteLine($"❌ Item still not found after modal for: {record.Item}");
                            continue; // Or throw
                        }
                    }
                    if (measurement == null)
                    {
                        measurement = await _measurementService.HandleUnknownMeasurement(record.Measurement);
                        if (measurement == null)
                        {
                            Debug.WriteLine($"❌ Measurement still not found after modal for: {record.Item}");
                            continue; // Or throw
                        }
                    }
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
            finally { file.IsProcessing = false; };
        }
    }
}
