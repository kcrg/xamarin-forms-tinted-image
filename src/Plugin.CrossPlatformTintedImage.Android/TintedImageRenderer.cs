using Android.Content;
using Android.Graphics;
using Plugin.CrossPlatformTintedImage.Abstractions;
using Plugin.CrossPlatformTintedImage.Android;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(handler: typeof(TintedImage), target: typeof(TintedImageRenderer))]
namespace Plugin.CrossPlatformTintedImage.Android
{
    public class TintedImageRenderer : ImageRenderer
    {
        public TintedImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            SetTint();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TintedImage.TintColorProperty.PropertyName || e.PropertyName == Image.SourceProperty.PropertyName)
            {
                SetTint();
            }
        }

        private void SetTint()
        {
            if (Control == null || Element == null)
            {
                return;
            }

            if (((TintedImage)Element).TintColor.Equals(Color.Transparent))
            {
                //Turn off tinting
                if (Control.ColorFilter != null)
                {
                    Control.ClearColorFilter();
                }

                return;
            }

            //Apply tint color
            PorterDuffColorFilter colorFilter = new PorterDuffColorFilter(((TintedImage)Element).TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.SetColorFilter(colorFilter);
        }
    }
}