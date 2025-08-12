using CommunityToolkit;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ShipApp.MVVM.ViewModels;
using ShipApp.MVVM.Views;
using ShipApp.Service;

namespace ShipApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<AppShell>();

            //Popups
            builder.Services.AddTransientPopup<AddItemPopup, AddItemViewModel>();
            builder.Services.AddTransientPopup<AddMeasurementPopup, AddMeasurementViewModel>();

            //Services
            builder.Services.AddScoped<ItemService>();
            builder.Services.AddScoped<ShipService>();
            builder.Services.AddScoped<FileUploadService>();
            builder.Services.AddScoped<MeasurementService>();
            builder.Services.AddScoped<OrderRecordService>();

            // ViewModels / Views
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomeView>();

            builder.Services.AddTransient<InventoryView>();
            //builder.Services.AddTransient<InventoryViewModel>();

            builder.Services.AddTransient<SettingsView>();
            //builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
