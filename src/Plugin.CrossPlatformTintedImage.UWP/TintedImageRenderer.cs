using CompositionProToolkit;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Plugin.CrossPlatformTintedImage.Abstractions;
using Plugin.CrossPlatformTintedImage.UWP;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Size = Windows.Foundation.Size;

[assembly: ExportRenderer(typeof(TintedImage), typeof(TintedImageRenderer))]
namespace Plugin.CrossPlatformTintedImage.UWP
{
    public class TintedImageRenderer : ImageRenderer
    {
        private CompositionEffectBrush effectBrush;
        private SpriteVisual spriteVisual;
        private IImageSurface imageSurface;
        private Compositor compositor;
        private ICompositionGenerator generator;

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            SetupCompositor();
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            try
            {
                if (effectBrush != null && e.PropertyName == TintedImage.TintColorProperty.PropertyName)
                {
                    if (((TintedImage)Element).TintColor == Color.Transparent)
                    {
                        //Turn off tinting - need to redraw brush
                        effectBrush = null;
                        spriteVisual = null;
                    }
                    else
                    {
                        SetTint(GetNativeColor(((TintedImage)Element).TintColor));
                        return;
                    }
                }

                bool needsResizing = e.PropertyName == VisualElement.XProperty.PropertyName ||
                                    e.PropertyName == VisualElement.YProperty.PropertyName ||
                                    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                                    e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                                    e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
                                    e.PropertyName == VisualElement.TranslationXProperty.PropertyName ||
                                    e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
                                    e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
                                    e.PropertyName == VisualElement.RotationXProperty.PropertyName ||
                                    e.PropertyName == VisualElement.RotationProperty.PropertyName ||
                                    e.PropertyName == VisualElement.AnchorXProperty.PropertyName ||
                                    e.PropertyName == VisualElement.AnchorYProperty.PropertyName;

                if (spriteVisual != null && imageSurface != null && needsResizing)
                {
                    //Resizing Sprite Visual and Image Surface

                    spriteVisual.Size = new Vector2((float)Element.Width, (float)Element.Height);
                    imageSurface.Resize(new Size(Element.Width, Element.Height));

                    return;
                }

                if (e.PropertyName == Image.SourceProperty.PropertyName || spriteVisual == null || (effectBrush == null && ((TintedImage)Element).TintColor != Color.Transparent))
                {
                    await CreateTintEffectBrushAsync(new Uri($"ms-appx:///{((FileImageSource)Element.Source).File}"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to create Tinted Image. Exception: {ex.Message}");
            }
        }

        private void SetTint(Windows.UI.Color color)
        {
            effectBrush?.Properties.InsertColor("colorSource.Color", color);
        }

        private async Task CreateTintEffectBrushAsync(Uri uri)
        {
            if (Control == null || Element == null || Element.Width < 0 || Element.Height < 0)
            {
                return;
            }

            SetupCompositor();

            spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Size = new Vector2((float)Element.Width, (float)Element.Height);

            imageSurface = await generator.CreateImageSurfaceAsync(uri, new Size(Element.Width, Element.Height), ImageSurfaceOptions.DefaultOptimized);
            CompositionSurfaceBrush surfaceBrush = compositor.CreateSurfaceBrush(imageSurface.Surface);

            CompositionBrush targetBrush = surfaceBrush;

            if (((TintedImage)Element).TintColor == Color.Transparent)
            {
                //Don't apply tint effect
                effectBrush = null;
            }
            else
            {
                //Set target brush to tint effect brush

                Windows.UI.Color nativeColor = GetNativeColor(((TintedImage)Element).TintColor);

                IGraphicsEffect graphicsEffect = new CompositeEffect
                {
                    Mode = CanvasComposite.DestinationIn,
                    Sources =
            {
                new ColorSourceEffect
                {
                    Name = "colorSource",
                    Color = nativeColor
                },
                new CompositionEffectSourceParameter("mask")
            }
                };

                CompositionEffectFactory effectFactory = compositor.CreateEffectFactory(graphicsEffect,
                    new[] { "colorSource.Color" });

                effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("mask", surfaceBrush);

                SetTint(nativeColor);

                targetBrush = effectBrush;
            }

            spriteVisual.Brush = targetBrush;
            ElementCompositionPreview.SetElementChildVisual(Control, spriteVisual);
        }

        private void SetupCompositor()
        {
            if (compositor != null && generator != null)
            {
                return;
            }

            compositor = ElementCompositionPreview.GetElementVisual(Control).Compositor;
            generator = compositor.CreateCompositionGenerator();
        }

        private static Windows.UI.Color GetNativeColor(Color color)
        {
            return Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }
    }
}