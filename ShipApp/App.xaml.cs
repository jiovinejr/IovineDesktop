using Google.Apis.Drive.v3;
using ShipApp.Core;
using System.Diagnostics;
using ShipApp.Data;

namespace ShipApp
{
    public partial class App : Application
    {
        private readonly HttpServer _httpServer = new HttpServer();
        public App()
        {
            InitializeComponent();

            AppDataStore.Instance.LoadData();

            MainPage = new AppShell();

            // Start HTTP server at app launch
            _httpServer.Start("http://100.125.51.23:5000/");
            _ = InitializeGoogleAsync();

        }

        private async Task InitializeGoogleAsync()
        { 
            var driveService = await GoogleAuthHelper.GetDriveServiceAsync();
            
        }
    }
}
