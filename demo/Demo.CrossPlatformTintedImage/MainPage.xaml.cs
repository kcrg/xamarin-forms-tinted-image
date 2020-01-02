using Xamarin.Forms;

namespace Demo.CrossPlatformTintedImage
{
    public partial class MainPage : ContentPage
    {
        private bool shouldTint = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void UpdateTint()
        {
            if (redSlider == null || greenSlider == null || blueSlider == null)
            {
                return;
            }

            tintedImage.TintColor = shouldTint ? Color.FromRgb((int)redSlider.Value, (int)greenSlider.Value, (int)blueSlider.Value) : Color.Transparent;
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            UpdateTint();
        }

        private void OnTintSwitchToggled(object sender, ToggledEventArgs e)
        {
            shouldTint = e.Value;
            UpdateTint();
        }
    }
}