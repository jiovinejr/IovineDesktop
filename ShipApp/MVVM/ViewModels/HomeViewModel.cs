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

                string fileName = $"{file.FileName}"; // or .xls depending on your actual format
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                await downloader.DownloadFileAsync(file.FileDriveId, savePath);
                Debug.WriteLine($"✅ File downloaded to: {savePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Download error: {ex.Message}");
            }
        }
    }
}
