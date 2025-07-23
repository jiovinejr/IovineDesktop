using ShipApp.Core;
using System.Diagnostics;

namespace ShipApp
{
    public partial class App : Application
    {
        private readonly HttpServer _httpServer = new HttpServer();
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Start HTTP server at app launch
            _httpServer.Start("http://100.125.51.23:5000/");
            _ = InitializeGoogleAsync();

        }

        private async Task InitializeGoogleAsync()
        {
            Debug.WriteLine("WERE HERE");
            var driveService = await GoogleAuthHelper.GetDriveServiceAsync();
        }
    }
}
