using FL_Note.Elements;
using FL_Note.Extensions;
using FL_Note.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.SubPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingTemplate : ContentPage
	{

        bool IsOnBackImage = false;

        SensorSpeed speed = SensorSpeed.UI;

        byte _backGroundindex = 0;
        private byte BackGroundindex
        {
            get
            {
                return _backGroundindex;
            }
            set
            {
                _backGroundindex = value;
                for(int i = 0; i < AllBackGround.Count; i++)
                {
                    AllBackGround[i].BorderColor = i == BackGroundindex ? Color.Blue : Color.Transparent;
                }
            }
        }

        List<ImageButton> AllBackGround = new List<ImageButton>();

        MainPage _mainPage;
        public MainPage mainPage
        {
            get
            {
                return _mainPage;
            }
            set
            {
                _mainPage = value;
                if (_mainPage != null)
                {
                    WorkingImage.WidthRequest = mainPage.DrawLayoutSize.X * 0.607;
                    WorkingImage.HeightRequest = (WorkingImage.WidthRequest + 40) * (mainPage.DrawLayoutSize.Y / mainPage.DrawLayoutSize.X) - 40;

                    Label label = showimagetext;
                    showimage.FontSize = mainPage.DrawLayoutSize.X * 0.0316;
                    screenshottime.FontSize = mainPage.DrawLayoutSize.X * 0.0316;
                    savescreenshot.FontSize = mainPage.DrawLayoutSize.X * 0.0316;
                    shock.FontSize = mainPage.DrawLayoutSize.X * 0.0316;
                    lookway.FontSize = mainPage.DrawLayoutSize.X * 0.0316;

                    Grid makeStart(bool makeGrid)
                    {
                        BoxView Line = new BoxView()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            HeightRequest = 30
                        };
                        BackGroundChoose.Children.Add(Line);

                        Grid grid = null;
                        if (makeGrid)
                        {
                            grid = new Grid();
                            //grid.BackgroundColor = Color.Blue;
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            BackGroundChoose.Children.Add(grid);
                        }
                        return grid;
                    }

                    ImageButton makeImageButton(byte[] image, Color color, Grid grid, int index)
                    {
                        ImageButton imageButton1 = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            VerticalOptions = index == 1 ? LayoutOptions.EndAndExpand : LayoutOptions.StartAndExpand,
                            BorderWidth = 5,
                            CornerRadius = 10
                        };
                        if(color != new Color())
                        {
                            imageButton1.BackgroundColor = color;
                        }

                        ImageSource source;
                        if(image != null)
                        {
                            Stream stream1 = new MemoryStream(App.ReSizeImage(image, mainPage.SizeForImage));
                            source = ImageSource.FromStream(() => stream1);
                        }
                        else
                        {
                            string assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
                            Stream stream2;
                            using (Stream stream = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly.GetManifestResourceStream(assemblyName + ".Images.BackGround.PickBackGround.png"))
                            using (var reader = new BinaryReader(stream))
                            {
                                stream2 = new MemoryStream(App.ReSizeImage(reader.ReadBytes((int)stream.Length), mainPage.SizeForImage));
                            }
                            source = ImageSource.FromStream(() => stream2);
                        }

                        imageButton1.WidthRequest = mainPage.DrawLayoutSize.X * 0.403;
                        imageButton1.HeightRequest = imageButton1.WidthRequest * (mainPage.DrawLayoutSize.Y / mainPage.DrawLayoutSize.X);
                        imageButton1.Source = source;
                        imageButton1.Clicked += BackGroundButton_Clicked;
                        if (grid != null)
                        {
                            Grid.SetColumn(imageButton1, index);
                            grid.Children.Add(imageButton1);
                        }
                        else
                        {
                            BackGroundChoose.Children.Add(imageButton1);
                        }
                        AllBackGround.Add(imageButton1);
                        return imageButton1;
                    }

                    object[] backGrounds = ((object[])_mainPage.data["backgroundimage"]);
                    for (int i = 0; i < backGrounds.Length / 2; i++)
                    {
                        Grid grid = makeStart(true);
                        makeImageButton((byte[])backGrounds[i * 2], new Color(), grid, 0);
                        makeImageButton((byte[])backGrounds[i * 2 + 1], new Color(), grid, 1);
                    }
                    if (backGrounds.Length % 2 == 1)
                    {
                        Grid grid = makeStart(true);
                        makeImageButton((byte[])backGrounds[backGrounds.Length - 1], new Color(), grid, 0);
                        makeImageButton(null, Color.FromHex("#F0F0F0"), grid, 1);
                    }
                    else
                    {
                        makeStart(false);
                        makeImageButton(null, Color.FromHex("#F0F0F0"), null, -1);
                    }
                }
            }
        }

        public ImageSource Image
        {
            get
            {
                return ((Image)((Grid)WorkingImage.Content).Children[1]).Source;
            }
            set
            {
                ((Image)((Grid)WorkingImage.Content).Children[1]).Source = value;
            }
        }

        public ImageSource BackImage
        {
            get
            {
                return ((Image)((Grid)WorkingImage.Content).Children[0]).Source;
            }
            set
            {
                ((Image)((Grid)WorkingImage.Content).Children[0]).Source = value;
            }
        }

        ShowTemplate _showTemplate;
        public ShowTemplate showTemplate
        {
            get
            {
                return _showTemplate;
            }
            set
            {
                _showTemplate = value;


                Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
                Magnetometer.Start(speed);


                BackImage = ShowTemplate.CopyImageSource(value.BackgroundImage);
                Image = ShowTemplate.CopyImageSource(value.Image);
                name.Text = value.Name;
                showimage.SelectedIndex = (byte)value.ShowImage;
                screenshottime.SelectedIndex = (byte)value.ScreenshotTime;
                savescreenshot.SelectedIndex = (byte)(value.SaveScreenshot ? 1 : 0);
                shock.SelectedIndex = (byte)(value.Shock ? 1 : 0);
                lookway.SelectedIndex = (byte)value.LookWay;

                Dictionary<string, object> show = (Dictionary<string, object>)((object[])mainPage.data["setdata"])[mainPage.ShowTemplateList.IndexOf(showTemplate)];

                magneticSlider.Value = Convert.ToDouble(show["magnetic"]);

                BackGroundindex = show["background"].GetType().Name == "Byte" ? (byte)show["background"] : (byte)((object[])_mainPage.data["backgroundimage"]).Length;
                IsOnBackImage = show["background"].GetType().Name != "Byte";
                if (IsOnBackImage) AllBackGround[AllBackGround.Count - 1].Source = ShowTemplate.CopyImageSource(value.BackgroundImage);
            }
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            var mdata = e.Reading;
            magneticLayout.IsVisible = lookway.SelectedIndex == (byte)ShowTemplate.LookWayEnum.磁力;
            if (magneticLayout.IsVisible)
            {
                double power = Math.Sqrt(Math.Pow(mdata.MagneticField.X, 2) + Math.Pow(mdata.MagneticField.Y, 2) + Math.Pow(mdata.MagneticField.Z, 2)) / 4;
                magnetic.Text = "磁場敏感度(當下數值): " + magneticSlider.Value.ToString("F2") + "(" + power.ToString("F2") + ")";
            }
        }

        public SettingTemplate ()
		{
			InitializeComponent ();

            //mainPage = ((MainPage)Navigation.NavigationStack[0]);
            if (mainPage != null)
            {
                WorkingImage.WidthRequest = mainPage.DrawLayoutSize.X * 0.729;
                WorkingImage.HeightRequest = (WorkingImage.WidthRequest + 40) * (mainPage.DrawLayoutSize.Y / mainPage.DrawLayoutSize.X) - 40;
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Dictionary<string, object> show = (Dictionary<string, object>)((object[])mainPage.data["setdata"])[mainPage.ShowTemplateList.IndexOf(showTemplate)];
            bool change = false;
            if(name.Text != showTemplate.Name)
            {
                show["name"] = name.Text;
                showTemplate.Name = name.Text;
                change = true;
            }
            if (Image != showTemplate.Image)
            {
                show["image"] = ShowTemplate.ImageSourceToBytes(Image);
                showTemplate.Image = ShowTemplate.CopyImageSource(Image);
                change = true;
            }
            if((show["background"].GetType() != ((byte)0).GetType() && IsOnBackImage) || BackGroundindex != (byte)show["background"])
            {
                if (BackGroundindex == AllBackGround.Count - 1)
                {
                    show["background"] = ShowTemplate.ImageSourceToBytes(BackImage);
                    Stream stream = new MemoryStream((byte[])show["background"]);
                    showTemplate.BackgroundImage = ImageSource.FromStream(() => stream);
                }
                else
                {
                    show["background"] = BackGroundindex;
                    showTemplate.BackgroundImage = ShowTemplate.CopyImageSource(BackImage);
                }
                change = true;
            }
            if (showimage.SelectedIndex != (byte)showTemplate.ShowImage)
            {
                show["showimage"] = showimage.SelectedIndex;
                showTemplate.ShowImage = (ShowTemplate.ShowImageEnum)showimage.SelectedIndex;
                change = true;
            }
            if (screenshottime.SelectedIndex != (byte)showTemplate.ScreenshotTime)
            {
                show["screenshottime"] = screenshottime.SelectedIndex;
                showTemplate.ScreenshotTime = (ShowTemplate.ScreenshotTimeEnum)screenshottime.SelectedIndex;
                change = true;
            }
            if ((savescreenshot.SelectedIndex == 1) != showTemplate.SaveScreenshot)
            {
                show["savescreenshot"] = savescreenshot.SelectedIndex == 1;
                showTemplate.SaveScreenshot = savescreenshot.SelectedIndex == 1;
                change = true;
            }
            if ((shock.SelectedIndex == 1) != showTemplate.Shock)
            {
                show["shock"] = shock.SelectedIndex == 1;
                showTemplate.Shock = shock.SelectedIndex == 1;
                change = true;
            }
            if (lookway.SelectedIndex != (byte)showTemplate.LookWay)
            {
                show["lookway"] = lookway.SelectedIndex;
                showTemplate.LookWay = (ShowTemplate.LookWayEnum)lookway.SelectedIndex;
                change = true;
            }
            if (magneticSlider.Value != Convert.ToDouble(show["magnetic"]))
            {
                show["magnetic"] = magneticSlider.Value;
                change = true;
            }
            if (change)
            {
                App.WriteData(mainPage.data);
            }
            Magnetometer.Stop();
            Navigation.PopAsync();
            Grid tabbedPage = mainPage.FindByName<Grid>("controler");
            tabbedPage.IsVisible = true;
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            EditPage editPage = new EditPage();
            editPage.settingTemplate = this;
            Navigation.PushAsync(editPage);
        }

        private void PictureButton_Clicked(object sender, EventArgs e)
        {
            App.photoLibrary.OpenGallery((data) =>
            {
                SkiaSharp.SKImage SKBack = SkiaSharp.SKImage.FromEncodedData(ShowTemplate.ImageSourceToBytes(BackImage));
                int fa = Factor(SKBack.Width, SKBack.Height);
                App.photoLibrary.CropPhoto(data, new System.Numerics.Vector2(SKBack.Width / fa, SKBack.Height / fa), new System.Numerics.Vector2(SKBack.Width, SKBack.Height), (data2) =>
                {
                    Stream stream = new MemoryStream(App.MergeImage(data2, new SkiaSharp.SKSize(SKBack.Width, SKBack.Height), ShowTemplate.ImageSourceToBytes(Image)));
                    Image = ImageSource.FromStream(() => stream);
                });
            });
        }

        private void BackGroundButton_Clicked(object sender, EventArgs e)
        {
            byte nowindex = (byte)AllBackGround.IndexOf((ImageButton)sender);

            void PickPhoto()
            {
                App.photoLibrary.OpenGallery((data) =>
                {
                    SkiaSharp.SKImage SKBack = SkiaSharp.SKImage.FromEncodedData(ShowTemplate.ImageSourceToBytes(BackImage));
                    int fa = Factor(SKBack.Width, SKBack.Height);
                    App.photoLibrary.CropPhoto(data, new System.Numerics.Vector2(SKBack.Width / fa, SKBack.Height / fa), new System.Numerics.Vector2(SKBack.Width, SKBack.Height), (data2) =>
                    {
                        Stream stream = new MemoryStream(data2);
                        ((ImageButton)sender).Source = ImageSource.FromStream(() => stream);
                        BackImage = ShowTemplate.CopyImageSource(((ImageButton)sender).Source);
                        BackGroundindex = nowindex;
                        IsOnBackImage = true;
                    });
                });
            }

            if (nowindex != BackGroundindex)
            {
                object[] backGrounds = ((object[])_mainPage.data["backgroundimage"]);
                if (nowindex != (byte)backGrounds.Length)
                {
                    Stream stream = new MemoryStream(App.ReSizeImage((byte[])backGrounds[nowindex], mainPage.SizeForImage));
                    BackImage = ImageSource.FromStream(() => stream);
                    BackGroundindex = nowindex;
                }
                else
                {
                    if (IsOnBackImage)
                    {
                        BackImage = ShowTemplate.CopyImageSource(((ImageButton)sender).Source);
                        BackGroundindex = nowindex;
                    }
                    else
                    {
                        PickPhoto();
                    }
                }
            }
            else if(IsOnBackImage)
            {
                PickPhoto();
            }
        }

        int Factor(int x, int y)
        {
            for (int i = x % y; i != 0; x = y, y = i, i = x % y) { }
            return y;
        }
    }
}