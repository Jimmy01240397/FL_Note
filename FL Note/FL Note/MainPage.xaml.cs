using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouchTracking;
using Xamarin.Forms;
//using SkiaSharp;

namespace FL_Note
{
    public partial class MainPage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        SKImage sKImage = null;
        int ControlButtonClickNum = 0;
        Stopwatch ControlButtonClickRate = new Stopwatch();
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        public MainPage()
        {
            InitializeComponent();
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
                if (sKImage != null || completedPaths.Count != 0)
                {
                    sKImage = args.Surface.Snapshot();
                }
                //canvas.Save();
                completedPaths.Clear();
                canvas.Clear();

                if (sKImage != null)
                {
                    canvas.DrawImage(sKImage, new SKPoint(0, 0));
                }
            }
            else
            {
                canvas.Clear();

                if (sKImage != null)
                {
                    canvas.DrawImage(sKImage, new SKPoint(0, 0));
                }

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

        void OnClearClicked(object sender, EventArgs e)
        {
            completedPaths.Clear();
            inProgressPaths.Clear();
            sKImage = null;
            canvasView.InvalidateSurface();
        }

        void OnControlerTripleClick(object sender, EventArgs e)
        {
            switch(ControlButtonClickNum)
            {
                case 0:
                    {
                        ControlButtonClickRate.Restart();
                        ControlButtonClickNum++;
                    }
                    break;
                case 1:
                    {
                        if(ControlButtonClickRate.ElapsedMilliseconds <= 1000)
                        {
                            ControlButtonClickNum++;
                        }
                        else
                        {
                            ControlButtonClickNum = 0;
                            ControlButtonClickRate.Stop();
                            ControlButtonClickRate.Reset();
                        }
                    }
                    break;
                case 2:
                    {
                        if (ControlButtonClickRate.ElapsedMilliseconds <= 1000)
                        {
                            Drawing.IsVisible = false;
                            controler.IsVisible = true;
                            ControlButtonClickNum = 0;
                        }
                        else
                        {
                            ControlButtonClickNum = 0;
                            ControlButtonClickRate.Stop();
                            ControlButtonClickRate.Reset();
                        }
                    }
                    break;
            }
        }

        void OnControlerClick(object sender, EventArgs e)
        {
            Drawing.IsVisible = true;
            controler.IsVisible = false;
        }
    }

    public class GradientLayout : StackLayout
    {
        string _colorsList = "";
        public string ColorsList
        {
            get
            {
                return _colorsList;
            }
            set
            {
                _colorsList = value;
                this.IsVisible = false;
                this.IsVisible = true;
            }
        }

        public Color[] Colors
        {
            get
            {
                string[] hex = ColorsList.Split(',');
                Color[] colors = new Color[hex.Length];

                for (int i = 0; i < hex.Length; i++)
                {
                    string xx = hex[i].Trim();
                    FieldInfo f_key = typeof(Color).GetField(hex[i].Trim());
                    if (f_key != null)
                    {
                        colors[i] = (Color)f_key.GetValue(null);
                    }
                    else
                    {
                        colors[i] = Color.FromHex(hex[i].Trim());
                    }
                }

                return colors;
            }
        }

        public GradientColorStackMode Mode { get; set; }
    }

    public enum GradientColorStackMode
    {
        ToRight,
        ToLeft,
        ToTop,
        ToBottom,
        ToTopLeft,
        ToTopRight,
        ToBottomLeft,
        ToBottomRight
    }
}
