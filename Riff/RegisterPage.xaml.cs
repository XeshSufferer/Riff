using Riff.ViewModels;

namespace Riff;

public partial class RegisterPage : ContentPage
{

	private readonly RegisterPageViewModel _vm;

	public RegisterPage(RegisterPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = vm;
	}

	protected override void OnDisappearing()
	{
		_vm.Dispose();
		base.OnDisappearing();
	}

    protected override void OnAppearing()
    {
		_vm.Sub();
        base.OnAppearing();
    }
}