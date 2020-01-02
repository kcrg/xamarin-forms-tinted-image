namespace Demo.CrossPlatformTintedImage.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new CrossPlatformTintedImage.App());
        }
    }
}