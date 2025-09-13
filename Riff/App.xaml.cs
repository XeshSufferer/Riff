namespace Riff
{
    public partial class App : Application
    {
        public App()
        {
            UserAppTheme = AppTheme.Dark;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}