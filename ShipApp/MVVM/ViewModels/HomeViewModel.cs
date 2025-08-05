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
                    Item item = _itemService.GetItemByOriginalName(record.Item);
                    if (item == null)
                    {
                        var tcs = new TaskCompletionSource<Item>();
                        var addItemPage = new AddItemPage(record.Item, tcs); // 👈 You pass the original name and TCS

                        // Show modal
                        await Shell.Current.Navigation.PushModalAsync(addItemPage);

                        // Wait for modal to resolve
                        item = await tcs.Task;
                        await Shell.Current.Navigation.PopModalAsync();
                        // Optional: reload item from DB to be sure it's persisted correctly
                        item = _itemService.GetItemByOriginalName(item.OriginalItemName);

                        if (item == null)
                        {
                            Debug.WriteLine($"❌ Item still not found after modal for: {record.Item}");
                            continue; // Or throw, depending on your flow
                        }
                        
                    }

                    // ✅ Now `item` is guaranteed to exist. You can proceed with DB inserts.
                    Debug.WriteLine($"✅ Found or added item: {item}");
                    // e.g., use `item.ItemId` to insert the record
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
