using ShipApp.MVVM.Models;
using ShipApp.MVVM.ViewModels;

namespace ShipApp.MVVM.Views;

public partial class AddMeasurementPage : ContentPage
{
    public AddMeasurementPage(string originalMeasurementName, TaskCompletionSource<Measurement> tcs)
    {
        InitializeComponent();
        BindingContext = new AddMeasurementViewModel(originalMeasurementName, tcs);
    }
}
