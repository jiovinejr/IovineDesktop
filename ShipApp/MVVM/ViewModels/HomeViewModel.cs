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
                    Debug.WriteLine(record.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Download error: {ex.Message}");
            }
        }
    }
}
