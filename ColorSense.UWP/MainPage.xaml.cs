namespace ColorSense.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new ColorSense.App());
        }
    }
}
