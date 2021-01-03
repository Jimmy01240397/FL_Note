using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FL_Note.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.Elements
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawLayout : StackLayout
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        SKImage sKImage = null;
        List<SKImage> imagebefores = new List<SKImage>();

        ChooseColorLayout chooseColorLayout = new ChooseColorLayout()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        //public NavigationPage navigationPage;
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 15,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            BlendMode = SKBlendMode.Src
        };

        public DrawLayout ()
		{
			InitializeComponent ();
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }
        SKPoint ConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        void OnBackViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear(SKColors.White);

            SKPaint back = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Gray,
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            for (int i = 0; i < (int)(canvasView.Height / 30); i++)
            {
                canvas.DrawLine(ConvertToPixel(new TouchTrackingPoint(10, i * 30 + 30)), ConvertToPixel(new TouchTrackingPoint((float)canvasView.Width - 10, i * 30 + 30)), back);
            }
        }
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;

            if (inProgressPaths.Count == 0)
            {
                if (sKImage == null)
                {
                    canvas.Clear();
                }
                else
                {
                    imagebefores.Add(sKImage);
                }
                sKImage = args.Surface.Snapshot();
                completedPaths.Clear();
                canvas.Clear();

                canvas.DrawImage(sKImage, new SKPoint(0, 0));
            }
            else
            {
                canvas.Clear();
                canvas.DrawImage(sKImage, new SKPoint(0, 0));

                foreach (SKPath path in completedPaths)
                {
                    canvas.DrawPath(path, paint);
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    canvas.DrawPath(path, paint);
                }
            }
        }

        void OnChooseColorClicked(object sender, EventArgs e)
        {
            if (ChooseColorButton.Parent == Children[0])
            {
                ((Layout<View>)Children[0]).Children.Remove(ChooseColorButton);
                ((Grid)chooseColorLayout.Children[0]).Children.Add(ChooseColorButton);
                ChooseColorButton.TranslationY = -70;
                ((Layout<View>)((ContentPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]).Content).Children.Add(chooseColorLayout);
                chooseColorLayout.StopChooseColor += StopChooseColor;
                chooseColorLayout.ChooseColor += ChooseColor;
            }
            else
            {
                chooseColorLayout.Children.Remove(ChooseColorButton);
                ChooseColorButton.TranslationY = -20;
                ((Layout<View>)Children[0]).Children.Add(ChooseColorButton);
                ((Layout<View>)((ContentPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]).Content).Children.Remove(chooseColorLayout);
            }
        }

        void StopChooseColor(object sender, EventArgs e)
        {
            chooseColorLayout.Children.Remove(ChooseColorButton);
            ChooseColorButton.TranslationY = -20;
            ((Layout<View>)Children[0]).Children.Add(ChooseColorButton);
            ((Layout<View>)((ContentPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]).Content).Children.Remove(chooseColorLayout);
        }

        void ChooseColor(object sender, EventArgs e)
        {
            paint.Color = ((Button)sender).BackgroundColor.ToSKColor();
            ChooseColorButton.BackgroundColor = ((Button)sender).BackgroundColor;
            ChooseColorButton.BorderColor = ((Button)sender).BackgroundColor == Color.Transparent || ((Button)sender).BackgroundColor == Color.White ? Color.Black : Color.White;
            string assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
            ChooseColorButton.Source = ImageSource.FromResource(assemblyName + ".Images." + (((Button)sender).BackgroundColor == Color.Transparent || ((Button)sender).BackgroundColor == Color.White ? "colorpaletteBlack.png" : "colorpalette.png"), typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            chooseColorLayout.Children.Remove(ChooseColorButton);
            ChooseColorButton.TranslationY = -20;
            ((Layout<View>)Children[0]).Children.Add(ChooseColorButton);
            ((Layout<View>)((ContentPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]).Content).Children.Remove(chooseColorLayout);
        }

        public void Clear()
        {
            completedPaths.Clear();
            inProgressPaths.Clear();
            imagebefores.Clear();
            sKImage = null;
            canvasView.InvalidateSurface();
        }
    }
}