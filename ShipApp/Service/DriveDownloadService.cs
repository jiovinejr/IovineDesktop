using Google.Apis.Drive.v3;
using Google.Apis.Download;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using NPOI.SS.Formula.Functions;

namespace ShipApp.Service
{
    internal class DriveDownloadService
    {
        private readonly DriveService _driveService;

        public DriveDownloadService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<MemoryStream> DownloadFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();

            request.MediaDownloader.ProgressChanged += progress =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Completed:
                        Debug.WriteLine("✅ Download complete.");
                        break;
                    case DownloadStatus.Failed:
                        Debug.WriteLine("❌ Download failed.");
                        break;
                }
            };

            await request.DownloadAsync(stream);

            Debug.WriteLine($"Downloading {stream.Length}");
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public async Task ListFiles()
        {
            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.PageSize = 10;
                listRequest.Fields = "files(id, name)";
                string folderId = "1IOEXjvIw9QIMYDXJmB0_akKt73xNLNmI";
                listRequest.Q = $"'{folderId}' in parents and trashed = false";
                listRequest.SupportsAllDrives = true;
                listRequest.IncludeItemsFromAllDrives = true;

                var files = await listRequest.ExecuteAsync();
                Debug.WriteLine($"📦 File count: {files.Files?.Count ?? 0}");

                foreach (var file in files.Files)
                {
                    Debug.WriteLine($"📄 {file.Name} — {file.Id}");
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            
        }
    }
}
