using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FL_Note.Extensions;
using FL_Note.Interface;
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
        public enum ChooseImage
        {
            View,
            BackGround
        }

        public Dictionary<long, SKPath> inProgressPaths { get; private set; } = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        SKImage viewImage = null;
        public SKImage ViewImage
        {
            get
            {
                return viewImage;
            }
            set
            {
                viewImage = value;
                newimage = true;
            }
        }
        public SKImage BackImage { get; set; } = null;
        List<SKImage> imagebefores = new List<SKImage>();
        bool newimage = false;

        SKSize _allViewSize = new SKSize();
        public SKSize AllViewSize
        {
            get
            {
                return _allViewSize;
            }
            set
            {
                _allViewSize = value;
                if (ViewSurface == null)
                {
                    ViewSurface = SKSurface.Create(new SKImageInfo((int)AllViewSize.Width, (int)AllViewSize.Height));
                }
                else
                {
                    if (value == new SKSize())
                    {
                        SKImage sKImage = ViewSurface.Snapshot();
                        SKBitmap map1 = SKBitmap.FromImage(sKImage);
                        ViewSurface = SKSurface.Create(new SKImageInfo((int)AllViewSize.Width, (int)AllViewSize.Height));
                        ViewSurface.Canvas.DrawBitmap(map1, SKRect.Create(new SKPoint(0, 0), AllViewSize));
                    }
                    else
                    {
                        SKImage sKImage = ViewSurface.Snapshot();
                        SKBitmap map1 = SKBitmap.FromImage(sKImage);
                        ViewSurface = SKSurface.Create(new SKImageInfo((int)canvasView.CanvasSize.Width, (int)canvasView.CanvasSize.Height));
                        ViewSurface.Canvas.DrawBitmap(map1, SKRect.Create(new SKPoint(0, 0), canvasView.CanvasSize));
                    }
                }
            }
        }

        SKSurface ViewSurface = null;

        public bool isClear { get; private set; } = true;

        public event TouchActionEventHandler TouchAction;

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

        public event EventHandler EndDraw;

        bool drawinit = false;
        public event EventHandler DrawInit;

        public bool CanRestore
        {
            get
            {
                return imagebefores.Count > 0;
            }
        }

        public DrawLayout ()
		{
			InitializeComponent ();
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(sender, args);
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
                    }
                    canvasView.InvalidateSurface();
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                    }
                    canvasView.InvalidateSurface();
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
            //canvas.Clear(SKColor.Parse("#FFF8F8"));

            /*SKPaint back = new SKPaint
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
            }*/
            if (BackImage != null)
            {
                canvas.DrawImage(BackImage, SKRect.Create(new SKPoint(0, 0), AllViewSize == new SKSize() ? backView.CanvasSize : AllViewSize));
            }
            BackImage = args.Surface.Snapshot();
        }
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if(!drawinit)
            {
                DrawInit?.Invoke(sender, args);
                drawinit = true;
            }

            SKCanvas canvas = args.Surface.Canvas;

            if(ViewSurface == null)
            {
                ViewSurface = SKSurface.Create(new SKImageInfo((int)canvasView.CanvasSize.Width, (int)canvasView.CanvasSize.Height));
            }

            SKPixmap viewpix = ViewSurface.PeekPixels();
            SKPixmap viewpix2 = args.Surface.PeekPixels();
            canvas.Clear();
            if (inProgressPaths.Count == 0)
            {
                if (viewImage == null)
                {
                    ViewSurface.Canvas.Clear();
                    if (CanRestore)
                    {
                        viewImage = imagebefores[imagebefores.Count - 1];
                        imagebefores.RemoveAt(imagebefores.Count - 1);
                    }
                    else
                    {
                        viewImage = ViewSurface.Snapshot();
                        //viewImage = App.MoveImage(viewImage, new SKPoint(0, (viewpix.Height - viewpix2.Height) / 2));
                        isClear = true;
                    }
                }
                else
                {
                    if (newimage)
                    {
                        newimage = false;
                    }
                    else
                    {
                        if (completedPaths.Count > 0)
                        {
                            imagebefores.Add(viewImage);
                            viewImage = ViewSurface.Snapshot();
                            //viewImage = App.MoveImage(viewImage, new SKPoint(0, (viewpix.Height - viewpix2.Height) / 2));
                        }
                    }
                    isClear = false;
                }
                completedPaths.Clear();
                ViewSurface.Canvas.Clear();

                ViewSurface.Canvas.DrawImage(viewImage, SKRect.Create(new SKPoint(0, 0), new SKSize(viewpix.Width, viewpix.Height)));

                canvas.DrawSurface(ViewSurface, new SKPoint(0, 0));
                EndDraw?.Invoke(sender, args);
            }
            else
            {
                ViewSurface.Canvas.Clear();
                ViewSurface.Canvas.DrawImage(viewImage, SKRect.Create(new SKPoint(0, 0), new SKSize(viewpix.Width, viewpix.Height)));
                canvas.DrawSurface(ViewSurface, new SKPoint(0, 0));

                foreach (SKPath path in completedPaths)
                {
                    ViewSurface.Canvas.DrawPath(path, paint);
                    canvas.DrawPath(path, paint);
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    ViewSurface.Canvas.DrawPath(path, paint);
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
            viewImage = null;
            canvasView.InvalidateSurface();
        }

        public void Restore()
        {
            if (CanRestore && inProgressPaths.Count == 0 && completedPaths.Count == 0)
            {
                viewImage = null;
                canvasView.InvalidateSurface();
            }
        }

        public void InvalidateSurface()
        {
            canvasView.InvalidateSurface();
        }

        public byte[] GetImageByte(SKEncodedImageFormat imageFormat, ChooseImage chooseImage)
        {
            byte[] data = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                SKImage sKImage = null;
                switch (chooseImage)
                {
                    case ChooseImage.View:
                        {
                            sKImage = viewImage;
                        }
                        break;
                    case ChooseImage.BackGround:
                        {
                            sKImage = BackImage;
                        }
                        break;
                }
                sKImage.Encode(imageFormat, 100).SaveTo(memStream);
                data = memStream.ToArray();
            }
            return data;
        }

        public async void Save(SKEncodedImageFormat imageFormat, ChooseImage chooseImage, string folder, string filename)
        {
            byte[] data = GetImageByte(imageFormat, chooseImage);
            if (data != null && data.Length != 0)
            {
                bool success = await App.photoLibrary.
                    SavePhotoAsync(data, folder, filename);
            }
        }
    }
}