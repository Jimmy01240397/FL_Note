using FL_Note.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Numerics;
using TouchTracking;
using System.Drawing;
using Image = Xamarin.Forms.Image;
using SkiaSharp.Views.Forms;
using FL_Note.Interface;

namespace FL_Note
{
    public partial class MainPage : ContentPage
    {
        SensorSpeed speed = SensorSpeed.UI;

        int ControlButtonClickNum = 0;
        Stopwatch ControlButtonClickRate = new Stopwatch();
        Stopwatch ChangeImageRate = new Stopwatch();
        Stopwatch LongTouch = new Stopwatch();
        TouchTrackingPoint LongTouchLocation;

        bool showimage = false;

        bool isClear = false;

        public Dictionary<string, object> data;

        public List<long> points = new List<long>();

        byte[] beforimage = null;

        byte[] FullPageimage = null;

        public ShowTemplate DeleteShow { get; set; } = null;

        public IList<View> ShowTemplateList
        {
            get
            {
                return ShowPages.Children;
            }
        }

        public SKSize SizeForImage
        {
            get
            {
                return new SKSize(drawlayout.FindByName<SKCanvasView>("canvasView").CanvasSize.Width, drawlayout.FindByName<SKCanvasView>("canvasView").CanvasSize.Height);
            }
        }

        public Vector2 DrawLayoutSize
        {
            get
            {
                return new Vector2((float)Bounds.Width, (float)Bounds.Height - 50);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
            OrientationSensor.Start(speed);
            Magnetometer.Start(speed);
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            byte[] text;

            using (Stream stream = assembly.GetManifestResourceStream("FL_Note.data.settingdata.dat"))
            using (var reader = new BinaryReader(stream))
            {
                text = reader.ReadBytes((int)stream.Length);
            }
            if (!File.Exists(App.fileName))
            {
                File.WriteAllBytes(App.fileName, text);
            }
            data = App.ReadData();

            foreach (Dictionary<string, object> show in (object[])data["setdata"])
            {
                if ((bool)show["enable"])
                {
                    byte[] back = show["background"].GetType().Name == "Byte" ? (byte[])((object[])data["backgroundimage"])[(byte)show["background"]] : (byte[])show["background"];
                    drawlayout.BackImage = SKImage.FromEncodedData(back);
                    break;
                }
            }

        }

        public void Reset()
        {
            beforimage = null;
            isClear = false;
        }

        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            if (!App.photoLibrary.isApplicationInTheBackground() && !isClear)
            {
                Vector3 angle = ToEulerAngles(e.Reading.Orientation);
                if ((angle.X > 175 || angle.X < -175) && (angle.Y < 5 && angle.Y > -5))
                {
                    if (beforimage == null)
                    {
                        Dictionary<string, object> nowshow = null;
                        foreach (Dictionary<string, object> show in (object[])data["setdata"])
                        {
                            if ((bool)show["enable"])
                            {
                                nowshow = show;
                                break;
                            }
                        }
                        if (!ChangeImageRate.IsRunning && ChangeImageRate.ElapsedMilliseconds == 0)
                        {
                            ChangeImageRate.Start();
                        }
                        else if (ChangeImageRate.IsRunning && ChangeImageRate.ElapsedMilliseconds > 2000)
                        {
                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.面朝下)
                            {
                                beforimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                                if ((bool)nowshow["savescreenshot"])
                                {
                                    SaveImage();
                                }
                                if ((bool)nowshow["shock"])
                                {
                                    Vibration.Vibrate();
                                }
                            }
                            if (nowshow["image"] != null)
                            {
                                drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                drawlayout.InvalidateSurface();
                                //drawlayout.Save(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View, "FL-Note", "test.png");
                            }
                            else
                            {
                                drawlayout.Clear();
                            }
                            ChangeImageRate.Stop();
                        }
                    }
                }
                else
                {
                    ChangeImageRate.Stop();
                    ChangeImageRate.Reset();
                }
            }
            else
            {
                ChangeImageRate.Stop();
                ChangeImageRate.Reset();
            }
            //Console.WriteLine($"Euler: X: {.X}, Y: {ToEulerAngles(data.Orientation).Y}, Z: {ToEulerAngles(data.Orientation).Z}"); //+-180,0,
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            if (beforimage != null)
            {
                Dictionary<string, object> nowshow = null;
                foreach (Dictionary<string, object> show in (object[])data["setdata"])
                {
                    if ((bool)show["enable"])
                    {
                        nowshow = show;
                        break;
                    }
                }

                if ((byte)nowshow["lookway"] == (byte)ShowTemplate.LookWayEnum.磁力)
                {
                    var mdata = e.Reading;
                    double power = Math.Sqrt(Math.Pow(mdata.MagneticField.X, 2) + Math.Pow(mdata.MagneticField.Y, 2) + Math.Pow(mdata.MagneticField.Z, 2)) / 4;
                    //Console.WriteLine(power);
                    Frame ShowImage = (byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.左上縮圖 ? TopImage : ButtomImage;
                    if (power >= Convert.ToDouble(nowshow["magnetic"]) && !showimage)
                    {
                        ShowImage.WidthRequest = DrawLayoutSize.X * 0.19;
                        ShowImage.HeightRequest = (ShowImage.WidthRequest + 40) * (DrawLayoutSize.Y / DrawLayoutSize.X) - 40;

                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                        {
                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                            {
                                FullPageimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                            }
                            drawlayout.ViewImage = SKImage.FromEncodedData(beforimage);
                            drawlayout.InvalidateSurface();
                        }
                        else
                        {
                            Stream BackStream = new MemoryStream(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround));
                            ((Image)((Grid)ShowImage.Content).Children[0]).Source = ImageSource.FromStream(() => BackStream);
                            Stream ViewStream = new MemoryStream(beforimage);
                            ((Image)((Grid)ShowImage.Content).Children[1]).Source = ImageSource.FromStream(() => ViewStream);
                            ShowImage.IsVisible = true;
                        }

                        showimage = true;
                    }
                    else if (power < Convert.ToDouble(nowshow["magnetic"]) && showimage)
                    {
                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                        {
                            if (nowshow["image"] != null)
                            {
                                if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                {
                                    drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                }
                                else
                                {
                                    drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                }
                                drawlayout.InvalidateSurface();
                            }
                            else
                            {
                                drawlayout.Clear();
                            }
                        }
                        else
                        {
                            ShowImage.IsVisible = false;
                        }
                        showimage = false;
                    }
                }
            }
        }

        void OnClearClicked(object sender, EventArgs e)
        {
            //drawlayout.Save(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround, "FL-Note", Guid.NewGuid().ToString() + ".png");
            Dictionary<string, object> nowshow = null;
            foreach (Dictionary<string, object> show in (object[])data["setdata"])
            {
                if ((bool)show["enable"])
                {
                    nowshow = show;
                    break;
                }
            }
            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.面朝下)
            {
                if (beforimage != null)
                {
                    beforimage = null;
                    isClear = true;
                }
                drawlayout.Clear();
            }
            else if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
            {
                if(beforimage == null && !drawlayout.isClear && !isClear)
                {
                    beforimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                    if((bool)nowshow["savescreenshot"])
                    {
                        SaveImage();
                    }
                    if ((bool)nowshow["shock"])
                    {
                        Vibration.Vibrate();
                    }
                    drawlayout.Clear();
                }
                else
                {
                    if (beforimage != null)
                    {
                        beforimage = null;
                        isClear = true;
                    }
                    drawlayout.Clear();
                }
            }
        }

        void SaveImage()
        {
            App.photoLibrary.SavePhotoAsync(App.MergeImage(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround), beforimage), "FL-Note", Guid.NewGuid().ToString() + ".png");
        }

        Vector3 ToEulerAngles(Quaternion q)
        {
            double sinr_cosp = +2.0 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = +1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            double roll = Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double pitch;
            double sinp = +2.0 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                pitch = sinp < 0 ? -Math.Abs(Math.PI / 2) : Math.Abs(Math.PI / 2); // use 90 degrees if out of range
            else
                pitch = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = +2.0 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = +1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
            double yaw = Math.Atan2(siny_cosp, cosy_cosp);
            return new Vector3((float)(roll / Math.PI * 180), (float)(pitch / Math.PI * 180), (float)(yaw / Math.PI * 180));
        }

        void OnControlerTripleClick(object sender, EventArgs e)
        {
            switch (ControlButtonClickNum)
            {
                case 0:
                    {
                        ControlButtonClickRate.Restart();
                        ControlButtonClickNum++;
                    }
                    break;
                case 1:
                    {
                        if (ControlButtonClickRate.ElapsedMilliseconds > 1000)
                        {
                            ControlButtonClickNum = 0;
                            ControlButtonClickRate.Stop();
                            ControlButtonClickRate.Reset();
                            ControlButtonClickRate.Restart();
                        }
                        ControlButtonClickNum++;
                    }
                    break;
                case 2:
                    {
                        if (ControlButtonClickRate.ElapsedMilliseconds <= 1000)
                        {
                            Drawing.IsVisible = false;
                            controler.IsVisible = true;

                            OrientationSensor.Stop();
                            Magnetometer.Stop();

                            if (ShowPages.Children.Count == 1)
                            {
                                foreach (Dictionary<string, object> show in (object[])data["setdata"])
                                {
                                    NewShowTemplate(show);
                                }
                                if(ShowPages.Children.Count == 2)
                                {
                                    ((ShowTemplate)ShowPages.Children[0]).FindByName<ImageButton>("DeleteButton").IsEnabled = false;
                                }
                            }

                            ControlButtonClickNum = 0;
                        }
                        else
                        {
                            ControlButtonClickNum = 1;
                            ControlButtonClickRate.Stop();
                            ControlButtonClickRate.Reset();
                            ControlButtonClickRate.Restart();
                        }
                    }
                    break;
            }
        }

        void NewShowTemplate(Dictionary<string, object> show)
        {
            ShowTemplate showTemplate1 = new ShowTemplate(this);
            showTemplate1.HorizontalOptions = LayoutOptions.FillAndExpand;
            showTemplate1.HeightRequest = 330;
            showTemplate1.IsChecked = (bool)show["enable"];
            showTemplate1.Name = (string)show["name"];

            if (show["image"] != null)
            {
                Stream stream = new MemoryStream(App.ReSizeImage((byte[])show["image"], SizeForImage));
                showTemplate1.Image = ImageSource.FromStream(() => stream);
            }

            Stream stream2 = new MemoryStream(App.ReSizeImage(show["background"].GetType().Name == "Byte" ? (byte[])((object[])data["backgroundimage"])[(byte)show["background"]] : (byte[])show["background"], SizeForImage));
            showTemplate1.BackgroundImage = ImageSource.FromStream(() => stream2);
            showTemplate1.ShowImage = (ShowTemplate.ShowImageEnum)((byte)show["showimage"]);
            showTemplate1.ScreenshotTime = (ShowTemplate.ScreenshotTimeEnum)((byte)show["screenshottime"]);
            showTemplate1.SaveScreenshot = (bool)show["savescreenshot"];
            showTemplate1.Shock = (bool)show["shock"];
            showTemplate1.LookWay = (ShowTemplate.LookWayEnum)((byte)show["lookway"]);
            ShowPages.Children.Insert(ShowPages.Children.Count - 1, showTemplate1);
        }

        void OnControlerClick(object sender, EventArgs e)
        {
            bool change = false;
            for(int i = 0; i < ShowPages.Children.Count - 1; i++)
            {
                ShowTemplate showTemplate = (ShowTemplate)ShowPages.Children[i];
                Dictionary<string, object> show = (Dictionary<string, object>)((object[])data["setdata"])[i];
                RadioButton radioButton = (showTemplate.FindByName<RadioButton>("LabelRadio"));
                if(radioButton.IsChecked != (bool)show["enable"])
                {
                    show["enable"] = radioButton.IsChecked;
                    change = true;
                }
            }
            if(change)
            {
                App.WriteData(data);
            }
            foreach (Dictionary<string, object> show in (object[])data["setdata"])
            {
                if ((bool)show["enable"])
                {
                    byte[] back = show["background"].GetType().Name == "Byte" ? (byte[])((object[])data["backgroundimage"])[(byte)show["background"]] : (byte[])show["background"];
                    drawlayout.BackImage = SKImage.FromEncodedData(back);
                    break;
                }
            }
            Drawing.IsVisible = true;
            controler.IsVisible = false;
            OrientationSensor.Start(speed);
            Magnetometer.Start(speed);
            //ShowPages.Children.Clear();
        }

        void ResetClick(object sender, EventArgs e)
        {
            OnReset.Children[1].WidthRequest = DrawLayoutSize.X * 0.875;
            OnReset.Children[1].HeightRequest = NewMod.Children[1].WidthRequest * 0.445;
            OnReset.IsVisible = true;
        }

        void OnResetClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text != "取消")
            {
                Reset();
            }
            OnReset.IsVisible = false;
        }

        void MakeNewClick(object sender, EventArgs e)
        {
            NewModName.Text = "";
            NewMod.Children[1].WidthRequest = DrawLayoutSize.X * 0.875;
            NewMod.Children[1].HeightRequest = NewMod.Children[1].WidthRequest * 0.445;
            NewMod.IsVisible = true;
        }

        void NewModClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text == "取消")
            {
                NewMod.IsVisible = false;
            }
            else if (!string.IsNullOrEmpty(NewModName.Text))
            {
                NewMod.IsVisible = false;

                byte[] newimage = null;
                using (SKSurface surface = SKSurface.Create(new SKImageInfo((int)SizeForImage.Width, (int)SizeForImage.Height)))
                using (MemoryStream memStream = new MemoryStream())
                {
                    surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                    newimage = memStream.GetBuffer();
                }

                Dictionary<string, object> newmod = new Dictionary<string, object>() { { "enable", false }, { "name", NewModName.Text }, { "image", newimage }, { "background", (byte)0 }, { "showimage", (byte)ShowTemplate.ShowImageEnum.左上縮圖 }, { "screenshottime", (byte)ShowTemplate.ScreenshotTimeEnum.面朝下 }, { "savescreenshot", false }, { "shock", false }, { "lookway", (byte)ShowTemplate.LookWayEnum.磁力 }, { "magnetic", 300 } };
                object[] alldata = (object[])data["setdata"];
                if(alldata.Length == 0)
                {
                    newmod["enable"] = true;
                }
                Array.Resize(ref alldata, alldata.Length + 1);
                alldata[alldata.Length - 1] = newmod;
                data["setdata"] = alldata;
                NewShowTemplate(newmod);
                if (ShowPages.Children.Count > 2)
                {
                    for(int i = 0; i < ShowPages.Children.Count - 1; i++)
                    {
                        ((ShowTemplate)ShowPages.Children[i]).FindByName<ImageButton>("DeleteButton").IsEnabled = true;
                    }
                }
                App.WriteData(data);
            }
        }

        void OnDeleteClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text != "取消")
            {
                int index = ShowPages.Children.IndexOf(DeleteShow);
                RadioButton radioButton = (DeleteShow.FindByName<RadioButton>("LabelRadio"));
                if(radioButton.IsChecked)
                {
                    if(ShowPages.Children.Count > 2)
                    {
                        radioButton.IsChecked = false;
                        if (index == ShowPages.Children.Count - 2)
                        {
                            ((ShowTemplate)ShowPages.Children[index - 1]).FindByName<RadioButton>("LabelRadio").IsChecked = true;
                        }
                        else
                        {
                            ((ShowTemplate)ShowPages.Children[index + 1]).FindByName<RadioButton>("LabelRadio").IsChecked = true;
                        }
                    }
                }
                List<object> alldata = new List<object>((object[])data["setdata"]);
                alldata.RemoveAt(index);
                data["setdata"] = alldata.ToArray();
                ShowPages.Children.Remove(DeleteShow);
                if (ShowPages.Children.Count == 2)
                {
                    ((ShowTemplate)ShowPages.Children[0]).FindByName<ImageButton>("DeleteButton").IsEnabled = false;
                }
                App.WriteData(data);
            }
            DeleteShow = null;
            OnDelete.IsVisible = false;
        }

        private void Drawlayout_TouchAction(object sender, TouchTracking.TouchActionEventArgs args)
        {
            if (beforimage != null)
            {
                TopImage.WidthRequest = DrawLayoutSize.X * 0.19;
                TopImage.HeightRequest = (TopImage.WidthRequest + 40) * (DrawLayoutSize.Y / DrawLayoutSize.X) - 40;
                ButtomImage.WidthRequest = DrawLayoutSize.X * 0.19;
                ButtomImage.HeightRequest = (ButtomImage.WidthRequest + 40) * (DrawLayoutSize.Y / DrawLayoutSize.X) - 40;

                Dictionary<string, object> nowshow = null;
                foreach (Dictionary<string, object> show in (object[])data["setdata"])
                {
                    if ((bool)show["enable"])
                    {
                        nowshow = show;
                        break;
                    }
                }

                Frame ShowImage = (byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.左上縮圖 ? TopImage : ButtomImage;

                if ((byte)nowshow["lookway"] == (byte)ShowTemplate.LookWayEnum.三指)
                {
                    switch (args.Type)
                    {
                        case TouchActionType.Pressed:
                            if (!points.Contains(args.Id))
                            {
                                points.Add(args.Id);
                                if (points.Count == 3)
                                {
                                    if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                    {
                                        if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                        {
                                            FullPageimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                                        }
                                        drawlayout.ViewImage = SKImage.FromEncodedData(beforimage);
                                        drawlayout.InvalidateSurface();
                                    }
                                    else
                                    {
                                        Stream BackStream = new MemoryStream(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround));
                                        ((Image)((Grid)ShowImage.Content).Children[0]).Source = ImageSource.FromStream(() => BackStream);
                                        Stream ViewStream = new MemoryStream(beforimage);
                                        ((Image)((Grid)ShowImage.Content).Children[1]).Source = ImageSource.FromStream(() => ViewStream);
                                        ShowImage.IsVisible = true;
                                    }
                                    showimage = true;
                                }
                                else if (points.Count > 3)
                                {
                                    if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                    {
                                        if (nowshow["image"] != null)
                                        {
                                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                            {
                                                drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                            }
                                            else
                                            {
                                                drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                            }
                                            drawlayout.InvalidateSurface();
                                        }
                                        else
                                        {
                                            drawlayout.Clear();
                                        }
                                    }
                                    else
                                    {
                                        ShowImage.IsVisible = false;
                                    }
                                }
                            }
                            break;

                        case TouchActionType.Moved:
                            break;

                        case TouchActionType.Released:
                            if (points.Contains(args.Id))
                            {
                                points.Remove(args.Id);
                                if (showimage)
                                {
                                    drawlayout.inProgressPaths.Remove(args.Id);
                                    if (points.Count == 3)
                                    {
                                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                        {
                                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                            {
                                                FullPageimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                                            }
                                            drawlayout.ViewImage = SKImage.FromEncodedData(beforimage);
                                            drawlayout.InvalidateSurface();
                                        }
                                        else
                                        {
                                            Stream BackStream = new MemoryStream(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround));
                                            ((Image)((Grid)ShowImage.Content).Children[0]).Source = ImageSource.FromStream(() => BackStream);
                                            Stream ViewStream = new MemoryStream(beforimage);
                                            ((Image)((Grid)ShowImage.Content).Children[1]).Source = ImageSource.FromStream(() => ViewStream);
                                            ShowImage.IsVisible = true;
                                        }
                                    }
                                    else if (points.Count < 3)
                                    {
                                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                        {
                                            if (nowshow["image"] != null)
                                            {
                                                if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                                {
                                                    drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                                }
                                                else
                                                {
                                                    drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                                }
                                                drawlayout.InvalidateSurface();
                                            }
                                            else
                                            {
                                                drawlayout.Clear();
                                            }
                                        }
                                        else
                                        {
                                            ShowImage.IsVisible = false;
                                        }
                                        if (points.Count == 0)
                                        {
                                            showimage = false;
                                        }
                                    }
                                }
                            }
                            break;

                        case TouchActionType.Cancelled:
                            if (points.Contains(args.Id))
                            {
                                points.Remove(args.Id);
                                if (showimage)
                                {
                                    drawlayout.inProgressPaths.Remove(args.Id);
                                    if (points.Count == 3)
                                    {
                                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                        {
                                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                            {
                                                FullPageimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                                            }
                                            drawlayout.ViewImage = SKImage.FromEncodedData(beforimage);
                                            drawlayout.InvalidateSurface();
                                        }
                                        else
                                        {
                                            Stream BackStream = new MemoryStream(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround));
                                            ((Image)((Grid)ShowImage.Content).Children[0]).Source = ImageSource.FromStream(() => BackStream);
                                            Stream ViewStream = new MemoryStream(beforimage);
                                            ((Image)((Grid)ShowImage.Content).Children[1]).Source = ImageSource.FromStream(() => ViewStream);
                                            ShowImage.IsVisible = true;
                                        }
                                    }
                                    else if (points.Count < 3)
                                    {
                                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                        {
                                            if (nowshow["image"] != null)
                                            {
                                                if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                                {
                                                    drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                                }
                                                else
                                                {
                                                    drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                                }
                                                drawlayout.InvalidateSurface();
                                            }
                                            else
                                            {
                                                drawlayout.Clear();
                                            }
                                        }
                                        else
                                        {
                                            ShowImage.IsVisible = false;
                                        }
                                        if (points.Count == 0)
                                        {
                                            showimage = false;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else if((byte)nowshow["lookway"] == (byte)ShowTemplate.LookWayEnum.長按)
                {
                    switch (args.Type)
                    {
                        case TouchActionType.Pressed:
                            if (!LongTouch.IsRunning)
                            {
                                points.Add(args.Id);
                                LongTouchLocation = args.Location;
                                LongTouch.Start();
                            }
                            break;

                        case TouchActionType.Moved:
                            //Console.WriteLine($"X:{args.Location.X - LongTouchLocation.X} Y:{args.Location.Y - LongTouchLocation.Y}");
                            if (points.Contains(args.Id))
                            {
                                if (Math.Sqrt(Math.Pow(args.Location.X - LongTouchLocation.X, 2) + Math.Pow(args.Location.Y - LongTouchLocation.Y, 2)) > 10)
                                {
                                    LongTouch.Stop();
                                }
                                if (LongTouch.IsRunning)
                                {
                                    if (LongTouch.ElapsedMilliseconds > 1000)
                                    {
                                        if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                        {
                                            if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                            {
                                                FullPageimage = drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View);
                                            }
                                            drawlayout.ViewImage = SKImage.FromEncodedData(beforimage);
                                            drawlayout.InvalidateSurface();
                                        }
                                        else
                                        {
                                            Stream BackStream = new MemoryStream(drawlayout.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround));
                                            ((Image)((Grid)ShowImage.Content).Children[0]).Source = ImageSource.FromStream(() => BackStream);
                                            Stream ViewStream = new MemoryStream(beforimage);
                                            ((Image)((Grid)ShowImage.Content).Children[1]).Source = ImageSource.FromStream(() => ViewStream);
                                            ShowImage.IsVisible = true;
                                        }
                                        showimage = true;
                                        LongTouch.Stop();
                                    }
                                }
                            }
                            break;

                        case TouchActionType.Released:
                            if (points.Contains(args.Id))
                            {
                                points.Remove(args.Id);
                                LongTouch.Stop();
                                LongTouch.Reset();
                            }
                            if (showimage)
                            {
                                drawlayout.inProgressPaths.Remove(args.Id);

                                if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                {
                                    if (nowshow["image"] != null)
                                    {
                                        if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                        {
                                            drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                        }
                                        else
                                        {
                                            drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                        }
                                        drawlayout.InvalidateSurface();
                                    }
                                    else
                                    {
                                        drawlayout.Clear();
                                    }
                                }
                                else
                                {
                                    ShowImage.IsVisible = false;
                                }
                                showimage = false;
                            }
                            break;

                        case TouchActionType.Cancelled:
                            if (points.Contains(args.Id))
                            {
                                points.Remove(args.Id);
                                LongTouch.Stop();
                                LongTouch.Reset();
                            }
                            if (showimage)
                            {
                                drawlayout.inProgressPaths.Remove(args.Id);
                                if ((byte)nowshow["showimage"] == (byte)ShowTemplate.ShowImageEnum.全螢幕)
                                {
                                    if (nowshow["image"] != null)
                                    {
                                        if ((byte)nowshow["screenshottime"] == (byte)ShowTemplate.ScreenshotTimeEnum.清除按鈕)
                                        {
                                            drawlayout.ViewImage = SKImage.FromEncodedData(FullPageimage);
                                        }
                                        else
                                        {
                                            drawlayout.ViewImage = SKImage.FromEncodedData((byte[])nowshow["image"]);
                                        }
                                        drawlayout.InvalidateSurface();
                                    }
                                    else
                                    {
                                        drawlayout.Clear();
                                    }
                                }
                                else
                                {
                                    ShowImage.IsVisible = false;
                                }
                                showimage = false;
                                LongTouch.Stop();
                                LongTouch.Reset();
                            }
                            break;
                    }
                }
            }
        }
    }
}
