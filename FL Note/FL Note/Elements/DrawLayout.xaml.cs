﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public enum ChooseImage
        {
            View,
            BackGround
        }

        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        public SKImage ViewImage { get; set; } = null;
        public SKImage BackImage { get; set; } = null;
        List<SKImage> imagebefores = new List<SKImage>();
        bool init = false;


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
                canvas.DrawImage(BackImage, new SKPoint(0, 0));
            }
            BackImage = args.Surface.Snapshot();
        }
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;

            if (inProgressPaths.Count == 0)
            {
                if (ViewImage == null)
                {
                    canvas.Clear();
                    if (CanRestore)
                    {
                        ViewImage = imagebefores[imagebefores.Count - 1];
                        imagebefores.RemoveAt(imagebefores.Count - 1);
                    }
                    else
                    {
                        ViewImage = args.Surface.Snapshot();
                        init = true;
                    }
                }
                else
                {
                    if(init)
                    {
                        imagebefores.Add(ViewImage);
                        ViewImage = args.Surface.Snapshot();
                    }
                    else
                    {
                        init = true;
                    }
                }
                completedPaths.Clear();
                canvas.Clear();

                canvas.DrawImage(ViewImage, new SKPoint(0, 0));

                EndDraw?.Invoke(sender, args);
            }
            else
            {
                canvas.Clear();
                canvas.DrawImage(ViewImage, new SKPoint(0, 0));

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
            ViewImage = null;
            canvasView.InvalidateSurface();
        }

        public void Restore()
        {
            if (CanRestore && inProgressPaths.Count == 0 && completedPaths.Count == 0)
            {
                ViewImage = null;
                canvasView.InvalidateSurface();
            }
        }

        public byte[] GetImageByte(SKEncodedImageFormat imageFormat, ChooseImage chooseImage)
        {
            byte[] data = null;
            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream))
            {
                SKImage sKImage = null;
                switch (chooseImage)
                {
                    case ChooseImage.View:
                        {
                            sKImage = ViewImage;
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
                bool success = await DependencyService.Get<IPhotoLibrary>().
                    SavePhotoAsync(data, folder, filename);
            }
        }
    }
}