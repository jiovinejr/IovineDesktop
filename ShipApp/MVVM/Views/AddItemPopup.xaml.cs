using ShipApp.MVVM.ViewModels;

namespace ShipApp.MVVM.Views;

public partial class AddItemPopup : ContentView
{
	public AddItemPopup(AddItemViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}