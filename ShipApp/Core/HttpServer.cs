using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using ShipApp.MVVM.Models;
using ShipApp.Service;

public class HttpServer
{
    private HttpListener? _listener;
    private bool _isRunning = false;

    public async void Start(string urlPrefix = "http://localhost:5000/")
    {
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException("HttpListener not supported on this platform.");
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://100.125.51.23:5000/");
        _listener.Start();
        _isRunning = true;

        Debug.WriteLine($"🚀 Listening on {urlPrefix}");

        while (_isRunning)
        {
            var context = await _listener.GetContextAsync();
            _ = HandleRequest(context);
        }
    }

    private async Task HandleRequest(HttpListenerContext context)
    {
        try
        {
            if (context.Request.HttpMethod == "POST")
            {
                using var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                string body = await reader.ReadToEndAsync();

                Debug.WriteLine($"📥 Received POST: {body}");

                //Deserialize JSON to FileUpload object
                var fileUpload = JsonSerializer.Deserialize<FileUpload>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Debug.WriteLine(fileUpload.ToString());
                if (fileUpload != null)
                {
                    fileUpload.IsProcessed = false;
                    var service = new FileUploadService();
                    service.InsertFile(fileUpload);
                    Debug.WriteLine("✅ File metadata inserted to DB.");
                }

                context.Response.StatusCode = 200;
                byte[] buffer = Encoding.UTF8.GetBytes("Received!");
                await context.Response.OutputStream.WriteAsync(buffer);
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = 405;
                context.Response.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
    }
}

