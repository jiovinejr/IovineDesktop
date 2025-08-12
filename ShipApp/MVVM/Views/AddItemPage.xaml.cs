using ShipApp.MVVM.ViewModels;
using ShipApp.MVVM.Models;
using CommunityToolkit.Maui.Views;

namespace ShipApp.MVVM.Views;

public partial class AddItemPage : Popup
{
    public AddItemPage(string originalItemName)
    {
        InitializeComponent();
    }
}
