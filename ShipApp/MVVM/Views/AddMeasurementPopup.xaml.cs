using ShipApp.MVVM.ViewModels;

namespace ShipApp.MVVM.Views;

public partial class AddMeasurementPopup : ContentView
{
    public AddMeasurementPopup(AddMeasurementViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
