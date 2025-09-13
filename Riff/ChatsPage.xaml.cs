using Riff.ViewModels;

namespace Riff;

public partial class ChatsPage : ContentPage
{

	private readonly ChatsPageViewModel _vm;

	public ChatsPage(ChatsPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		_vm = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

	protected override void OnDisappearing()
	{
		_vm.Dispose();
		base.OnDisappearing();
	}
}