using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShipApp.Core
{
    public static class GoogleAuthHelper
    {
        public static async Task<DriveService> GetDriveServiceAsync()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "client_secrets.json");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.Scope.DriveFile },
                "user",
                CancellationToken.None);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Iovine Desktop"
            });
        }

        public static async Task<GmailService> GetGmailServiceAsync()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "client_secrets.json");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { GmailService.Scope.GmailSend },
                "user",
                CancellationToken.None);

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "ShipApp Desktop"
            });
        }
    }
}
