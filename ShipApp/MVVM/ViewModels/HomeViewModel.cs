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
                string fileName = $"{file.FileName}";
                MemoryStream stream = new MemoryStream();
                stream = await downloader.DownloadFileAsync(file.FileDriveId);
                var rdf = new ReadDataFile(stream, file.FileName);
                List<ExcelRecord> records = rdf.CreateExcelOrderList();
                Debug.WriteLine($"✅ File downloaded: {file.FileName}");

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
                        item = await HandleUnknownItemAsync(record.Item);
                        if (item == null)
                        {
                            Debug.WriteLine($"❌ Item still not found after modal for: {record.Item}");
                            continue; // Or throw
                        }
                    }
                    if (measurement == null)
                    {
                        measurement = await HandleUnknownMeasurement(record.Measurement);
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

        private async Task<Measurement?> HandleUnknownMeasurement(string originalMeasurementName)
        {
            try
            {
                var tcs = new TaskCompletionSource<Measurement>();
                var addMeasurementPage = new AddMeasurementPage(originalMeasurementName, tcs);

                await Shell.Current.Navigation.PushModalAsync(addMeasurementPage);

                var measurement = await tcs.Task;
                await Shell.Current.Navigation.PopModalAsync();

                return measurement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to handle unknown measurement: {ex.Message}");
                return null;
            }
        }

        private async Task<Item?> HandleUnknownItemAsync(string originalItemName)
        {
            try
            {
                var tcs = new TaskCompletionSource<Item>();
                var addItemPage = new AddItemPage(originalItemName, tcs);

                await Shell.Current.Navigation.PushModalAsync(addItemPage);

                var item = await tcs.Task;
                await Shell.Current.Navigation.PopModalAsync();

                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to handle unknown item: {ex.Message}");
                return null;
            }
        }

    }
}
