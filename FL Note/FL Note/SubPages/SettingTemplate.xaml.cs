using FL_Note.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.SubPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingTemplate : ContentPage
	{
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
                    WorkingImage.WidthRequest = 250;
                    WorkingImage.HeightRequest = (WorkingImage.WidthRequest + 40) * ((_mainPage.Bounds.Height - 50) / _mainPage.Bounds.Width) - 40;

                    object[] backGrounds = ((object[])_mainPage.data["backgroundimage"]);
                    for(int i = 0; i < backGrounds.Length / 2; i++)
                    {
                        BoxView Line = new BoxView()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            HeightRequest = 30
                        };
                        BackGroundChoose.Children.Add(Line);

                        Grid grid = new Grid();
                        //grid.BackgroundColor = Color.Blue;
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        BackGroundChoose.Children.Add(grid);
                        ImageButton imageButton1 = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            BorderWidth = 5
                        };
                        ImageButton imageButton2 = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            BorderWidth = 5
                        };
                        Stream stream1 = new MemoryStream((byte[])backGrounds[i * 2]);
                        imageButton1.WidthRequest = 166;
                        imageButton1.HeightRequest = imageButton1.WidthRequest * ((_mainPage.Bounds.Height - 50) / _mainPage.Bounds.Width);
                        imageButton1.Source = ImageSource.FromStream(() => stream1);
                        imageButton1.Clicked += BackGroundButton_Clicked;
                        Grid.SetColumn(imageButton1, 0);
                        grid.Children.Add(imageButton1);
                        AllBackGround.Add(imageButton1);

                        Stream stream2 = new MemoryStream((byte[])backGrounds[i * 2 + 1]);
                        imageButton2.WidthRequest = 166;
                        imageButton2.HeightRequest = imageButton2.WidthRequest * ((_mainPage.Bounds.Height - 50) / _mainPage.Bounds.Width);
                        imageButton2.Source = ImageSource.FromStream(() => stream2);
                        imageButton2.Clicked += BackGroundButton_Clicked;
                        Grid.SetColumn(imageButton2, 1);
                        grid.Children.Add(imageButton2);
                        AllBackGround.Add(imageButton2);
                    }
                    if (backGrounds.Length % 2 == 1)
                    {
                        BoxView Line = new BoxView()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            HeightRequest = 30
                        };
                        BackGroundChoose.Children.Add(Line);

                        ImageButton imageButton1 = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Start,
                            BorderWidth = 5
                        };
                        Stream stream1 = new MemoryStream((byte[])backGrounds[backGrounds.Length - 1]);
                        imageButton1.WidthRequest = 166;
                        imageButton1.HeightRequest = imageButton1.WidthRequest * ((_mainPage.Bounds.Height - 50) / _mainPage.Bounds.Width);
                        imageButton1.Source = ImageSource.FromStream(() => stream1);
                        imageButton1.Clicked += BackGroundButton_Clicked;
                        BackGroundChoose.Children.Add(imageButton1);
                        AllBackGround.Add(imageButton1);
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

                BackImage = ShowTemplate.CopyImageSource(value.BackgroundImage);
                Image = ShowTemplate.CopyImageSource(value.Image);
                name.Text = value.Name;
                showimage.SelectedIndex = (byte)value.ShowImage;
                screenshottime.SelectedIndex = (byte)value.ScreenshotTime;
                savescreenshot.SelectedIndex = (byte)(value.SaveScreenshot ? 1 : 0);
                shock.SelectedIndex = (byte)(value.Shock ? 1 : 0);
                lookway.SelectedIndex = (byte)value.LookWay;

                Dictionary<string, object> show = (Dictionary<string, object>)((object[])mainPage.data["setdata"])[mainPage.ShowTemplateList.IndexOf(showTemplate)];
                BackGroundindex = show["background"].GetType().Name == "Byte" ? (byte)show["background"] : (byte)((object[])_mainPage.data["backgroundimage"]).Length;
            }
        }

        public SettingTemplate ()
		{
			InitializeComponent ();

            //mainPage = ((MainPage)Navigation.NavigationStack[0]);
            if (mainPage != null)
            {
                WorkingImage.WidthRequest = 300;
                WorkingImage.HeightRequest = (WorkingImage.WidthRequest + 40) * ((mainPage.Bounds.Height - 50) / mainPage.Bounds.Width) - 40;
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
            if(BackGroundindex != (byte)show["background"])
            {
                show["background"] = BackGroundindex;
                showTemplate.BackgroundImage = ShowTemplate.CopyImageSource(BackImage);
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
            if(change)
            {
                App.WriteData(mainPage.data);
            }
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

        }

        private void BackGroundButton_Clicked(object sender, EventArgs e)
        {
            byte nowindex = (byte)AllBackGround.IndexOf((ImageButton)sender);
            if(nowindex != BackGroundindex)
            {
                object[] backGrounds = ((object[])_mainPage.data["backgroundimage"]);
                if (nowindex != (byte)backGrounds.Length)
                {
                    Stream stream = new MemoryStream((byte[])backGrounds[nowindex]);
                    BackImage = ImageSource.FromStream(() => stream);
                    BackGroundindex = nowindex;
                }
                else
                {

                }
            }
        }
    }
}