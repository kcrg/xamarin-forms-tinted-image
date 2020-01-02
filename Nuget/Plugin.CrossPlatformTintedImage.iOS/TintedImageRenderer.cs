using Plugin.CrossPlatformTintedImage.Abstractions;
using Plugin.CrossPlatformTintedImage.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TintedImage), typeof(TintedImageRenderer))]
namespace Plugin.CrossPlatformTintedImage.iOS
{
    public class TintedImageRenderer : ImageRenderer
    {
        public static new void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            SetTint();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TintedImage.TintColorProperty.PropertyName || e.PropertyName == TintedImage.SourceProperty.PropertyName)
            {
                SetTint();
            }
        }

        private void SetTint()
        {
            if (Control?.Image == null || Element == null)
            {
                return;
            }

            if (((TintedImage)Element).TintColor == Color.Transparent)
            {
                //Turn off tinting
                Control.Image = Control.Image.ImageWithRenderingMode(UIKit.UIImageRenderingMode.Automatic);
                Control.TintColor = null;
            }
            else
            {
                //Apply tint color
                Control.Image = Control.Image.ImageWithRenderingMode(UIKit.UIImageRenderingMode.AlwaysTemplate);
                Control.TintColor = ((TintedImage)Element).TintColor.ToUIColor();
            }
        }
    }
}