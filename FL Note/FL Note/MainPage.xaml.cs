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

namespace FL_Note
{
    public partial class MainPage : ContentPage
    {
        int ControlButtonClickNum = 0;
        Stopwatch ControlButtonClickRate = new Stopwatch();
        public Dictionary<string, object> data;

        public MainPage()
        {
            this.InitializeComponent();

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
                if((bool)show["enable"])
                {
                    byte[] back = show["background"].GetType().Name == "Byte" ? (byte[])((object[])data["backgroundimage"])[(byte)show["background"]] : (byte[])show["background"];
                    drawlayout.BackImage = SKImage.FromEncodedData(back);
                    break;
                }
            }
        }

        void OnClearClicked(object sender, EventArgs e)
        {
            //drawlayout.Save(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.BackGround, "FL-Note", Guid.NewGuid().ToString() + ".png");
            drawlayout.Clear();
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
                        if(ControlButtonClickRate.ElapsedMilliseconds > 1000)
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


                            if (ShowPages.Children.Count == 0)
                            {
                                foreach (Dictionary<string, object> show in (object[])data["setdata"])
                                {
                                    ShowTemplate showTemplate1 = new ShowTemplate(this);
                                    showTemplate1.HorizontalOptions = LayoutOptions.FillAndExpand;
                                    showTemplate1.HeightRequest = 330;
                                    showTemplate1.IsChecked = (bool)show["enable"];
                                    showTemplate1.Name = (string)show["name"];

                                    Stream stream = new MemoryStream((byte[])show["image"]);
                                    showTemplate1.Image = ImageSource.FromStream(() => stream);

                                    Stream stream2 = new MemoryStream(show["background"].GetType().Name == "Byte" ? (byte[])((object[])data["backgroundimage"])[(byte)show["background"]] : (byte[])show["background"]);
                                    showTemplate1.BackgroundImage = ImageSource.FromStream(() => stream2);
                                    showTemplate1.ShowImage = (ShowTemplate.ShowImageEnum)((byte)show["showimage"]);
                                    showTemplate1.ScreenshotTime = (ShowTemplate.ScreenshotTimeEnum)((byte)show["screenshottime"]);
                                    showTemplate1.SaveScreenshot = (bool)show["savescreenshot"];
                                    showTemplate1.Shock = (bool)show["shock"];
                                    showTemplate1.LookWay = (ShowTemplate.LookWayEnum)((byte)show["lookway"]);
                                    ShowPages.Children.Add(showTemplate1);
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

        void OnControlerClick(object sender, EventArgs e)
        {
            Drawing.IsVisible = true;
            controler.IsVisible = false;
            //ShowPages.Children.Clear();
        }
    }
}
