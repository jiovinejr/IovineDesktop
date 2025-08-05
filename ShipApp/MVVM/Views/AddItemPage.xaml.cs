using ShipApp.MVVM.ViewModels;
using ShipApp.MVVM.Models;

namespace ShipApp.MVVM.Views;

public partial class AddItemPage : ContentPage
{
    public AddItemPage(string originalItemName, TaskCompletionSource<Item> tcs)
    {
        InitializeComponent();

        BindingContext = new AddItemViewModel(originalItemName, tcs);
    }
}
