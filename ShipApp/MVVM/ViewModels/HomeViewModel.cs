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

namespace ShipApp.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<FileUpload> FilesToProcess { get; set; }
        public ICommand DownloadCommand { get; }
        

        public HomeViewModel() 
        {
            var uploadsService = new FileUploadService();
            FilesToProcess = uploadsService.GetFilesToProcess();

            DownloadCommand = new RelayCommand<FileUpload>(async (file) => await DownloadFileAsync(file));

        }


        private async Task DownloadFileAsync(FileUpload file)
        {
            try
            {
                var driveService = await GoogleAuthHelper.GetDriveServiceAsync();
                var itemService = new ItemService();
                var measurementService = new MeasurementService();
                var downloader = new DriveDownloadService(driveService);
                Debug.WriteLine(file.ToString());

                string fileName = $"{file.FileName}";
                MemoryStream stream = new MemoryStream();
                stream = await downloader.DownloadFileAsync(file.FileDriveId);
                var rdf = new ReadDataFile(stream, file.FileName);
                List<ExcelRecord> records = rdf.CreateExcelOrderList();
                Debug.WriteLine($"✅ File downloaded: {file.FileName}");
                foreach (ExcelRecord record in records)
                {
                    Item item = itemService.GetItemByOriginalName(record.Item);
                    Measurement measurement = measurementService.GetMeasurementObjectByOriginalName(record.Measurement);
                    if (item != null)
                    {
                        Debug.WriteLine(item.ToString());
                    }
                    else
                    {
                        Debug.WriteLine($"Item {record.Item} Not Found");
                    }
                    if (measurement != null)
                    {
                        Debug.WriteLine(measurement.ToString());
                    }
                    else
                    {
                        Debug.WriteLine($"Measurement {record.Measurement} Not Found.");
                    }

                }

                string newName = records[0].ShipName;
                await downloader.RenameDriveFile(file.FileDriveId, newName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Download error: {ex.Message}");
            }
        }
    }
}
