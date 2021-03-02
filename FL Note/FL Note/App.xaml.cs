using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using UnityNetwork;
using SkiaSharp;
using FL_Note.Elements;
using FL_Note.Interface;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FL_Note
{
    public partial class App : Application
    {
        public static readonly string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "settingdata.dat");

        public static IPhotoLibrary photoLibrary { get; set; } 

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.White,
                BarTextColor = Color.Blue
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static Dictionary<string, object> ReadData()
        {
            Dictionary<string, object> output = null;
            if (File.Exists(fileName))
            {
                byte[] data = File.ReadAllBytes(fileName);
                output = (Dictionary<string, object>)SerializationData.ToObject(data);
            }
            return output;
        }

        public static void WriteData(Dictionary<string, object> input)
        {
            if (File.Exists(fileName))
            {
                byte[] data = SerializationData.ToBytes(input);
                File.WriteAllBytes(fileName, data);
            }
        }

        public static byte[] MergeImage(byte[] A, byte[] B)
        {
            return MergeImage(A, new SKSize(), B, new SKSize());
        }

        public static byte[] MergeImage(byte[] A, SKSize ASize, byte[] B)
        {
            return MergeImage(A, ASize, B, new SKSize());
        }

        public static byte[] MergeImage(byte[] A, byte[] B, SKSize BSize)
        {
            return MergeImage(A, new SKSize(), B, BSize);
        }

        public static byte[] MergeImage(byte[] A, SKSize ASize, byte[] B, SKSize BSize)
        {
            byte[] ans = null;

            SKBitmap map1 = SKBitmap.FromImage(SKImage.FromEncodedData(A));

            SKBitmap map2 = SKBitmap.FromImage(SKImage.FromEncodedData(B));

            using (SKSurface surface = SKSurface.Create(new SKImageInfo(
                Math.Max(
                    ASize != new SKSize() ? (int)ASize.Width : map1.Width, 
                    BSize != new SKSize() ? (int)BSize.Width : map2.Width), 
                Math.Max(
                    ASize != new SKSize() ? (int)ASize.Height : map1.Height, 
                    BSize != new SKSize() ? (int)BSize.Height : map2.Height))))
            using (MemoryStream memStream = new MemoryStream())
            {
                if(ASize != new SKSize())
                {
                    surface.Canvas.DrawBitmap(map1, SKRect.Create(new SKPoint(0, 0), ASize));
                }
                else
                {
                    surface.Canvas.DrawBitmap(map1, new SKPoint(0, 0));
                }
                if (BSize != new SKSize())
                {
                    surface.Canvas.DrawBitmap(map2, SKRect.Create(new SKPoint(0, 0), BSize));
                }
                else
                {
                    surface.Canvas.DrawBitmap(map2, new SKPoint(0, 0));
                }
                SKImage sKImage = surface.Snapshot();
                sKImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                ans = memStream.GetBuffer();
            }
            return ans;
        }

        public static byte[] ReSizeImage(byte[] image, SKSize size)
        {
            byte[] ans = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                SKBitmap map1 = SKBitmap.FromImage(SKImage.FromEncodedData(image));
                if (map1.Width == size.Width && map1.Height == size.Height)
                {
                    ans = image;
                }
                else
                {
                    map1 = map1.Resize(new SKImageInfo((int)size.Width, (int)size.Height), SKFilterQuality.High);
                    map1.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                    ans = memStream.ToArray();
                }
            }
            return ans;
        }


        public static byte[] MoveImage(byte[] image, SKPoint point)
        {
            byte[] ans = null;
            SKBitmap map1 = SKBitmap.FromImage(SKImage.FromEncodedData(image));
            using (SKSurface surface = SKSurface.Create(new SKImageInfo(map1.Width, map1.Height)))
            using (MemoryStream memStream = new MemoryStream())
            {
                surface.Canvas.DrawBitmap(map1, SKRect.Create(point, new SKSize(map1.Width, map1.Height)));
                map1.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                ans = memStream.ToArray();
            }
            return ans;
        }
        public static SKImage MoveImage(SKImage image, SKPoint point)
        {
            SKImage ans;
            using (SKSurface surface = SKSurface.Create(new SKImageInfo(image.Width, image.Height)))
            {
                surface.Canvas.DrawImage(image, SKRect.Create(point, new SKSize(image.Width, image.Height)));
                ans = surface.Snapshot();
            }
            return ans;
        }
    }
}
