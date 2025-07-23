using Google.Apis.Drive.v3;
using Google.Apis.Download;
using System.IO;
using System.Threading.Tasks;

namespace ShipApp.Service
{
    internal class DriveDownloadService
    {
        private readonly DriveService _driveService;

        public DriveDownloadService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task DownloadFileAsync(string fileId, string savePath)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();

            request.MediaDownloader.ProgressChanged += progress =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Completed:
                        Console.WriteLine("✅ Download complete.");
                        break;
                    case DownloadStatus.Failed:
                        Console.WriteLine("❌ Download failed.");
                        break;
                }
            };

            await request.DownloadAsync(stream);

            // Save to file
            using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
        }
    }
}
