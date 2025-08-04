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

                string fileName = $"{file.FileName}";
                MemoryStream stream = new MemoryStream();
                stream = await downloader.DownloadFileAsync(file.FileDriveId);
                var rdf = new ReadDataFile(stream, file.FileName);
                List<ExcelRecord> records = rdf.CreateExcelOrderList();
                Debug.WriteLine($"✅ File downloaded: {file.FileName}");

                Ship ship = new Ship(records[0].ShipName);
                ship = _shipService.AddNewShip(ship);
                Debug.WriteLine(ship.ToString());

                

                //foreach (ExcelRecord record in records)
                //{
                //    Debug.WriteLine(record.ToString());
                //}

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
