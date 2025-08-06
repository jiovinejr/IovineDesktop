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
using ShipApp.Models;
using ShipApp.MVVM.Views;

namespace ShipApp.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<FileUpload> FilesToProcess { get; set; }
        public ICommand DownloadCommand { get; }

        private readonly ItemService _itemService;
        private readonly ShipService _shipService;
        private readonly FileUploadService _fileUploadService;
        private readonly MeasurementService _measurementService;


        public HomeViewModel()
        {
            _fileUploadService = new FileUploadService();
            _measurementService = new MeasurementService();
            _shipService = new ShipService();
            _itemService = new ItemService();


            FilesToProcess = _fileUploadService.GetFilesToProcess();

            DownloadCommand = new RelayCommand<FileUpload>(
                async (file) => await DownloadFileAsync(file),
                (file) => file != null && !file.IsProcessing
                );

        }


        private async Task DownloadFileAsync(FileUpload file)
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
                //Ship ship = new Ship(records[0].ShipName);
                //ship = _shipService.AddNewShip(ship);

                //Loop Through excel file
                foreach (ExcelRecord record in records)
                {
                    //Handle Item
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

                    Debug.WriteLine($"✅ Found or added item: {item}");
                    Debug.WriteLine(measurement.ToString());
                }

                //string newName = records[0].ShipName;
                //await downloader.RenameDriveFile(file.FileDriveId, newName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Download error: {ex.Message}");
            }
            finally { file.IsProcessing = false; };
        }
    }
}
