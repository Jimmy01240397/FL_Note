using FL_Note.SubPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.Elements
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowTemplate : StackLayout
	{
        public enum ShowImageEnum
        {
            左上縮圖,
            右下縮圖,
            全螢幕
        }

        public enum ScreenshotTimeEnum
        {
            面朝下,
            清除按鈕
        }

        public enum LookWayEnum
        {
            三指,
            長按,
            磁力
        }

        MainPage mainPage;

        public int index { get; set; }

        public bool IsChecked
        {
            get
            {
                return LabelRadio.IsChecked;
            }
            set
            {
                LabelRadio.IsChecked = value;
            }
        }

        public string Name
        {
            get
            {
                return LabelRadio.Text;
            }
            set
            {
                LabelRadio.Text = value;
            }
        }

        public ImageSource Image
        {
            get
            {
                return ((Image)((Grid)MagicImage.Content).Children[1]).Source;
            }
            set
            {
                ((Image)((Grid)MagicImage.Content).Children[1]).Source = value;
            }
        }

        public ImageSource BackgroundImage
        {
            get
            {
                return ((Image)BackGroundImage.Content).Source;
            }
            set
            {
                ((Image)BackGroundImage.Content).Source = value;
                ((Image)((Grid)MagicImage.Content).Children[0]).Source = CopyImageSource(value);
            }
        }

        public ShowImageEnum ShowImage
        {
            get
            {
                return (ShowImageEnum)Enum.Parse(typeof(ShowImageEnum), showimage.Text);
            }
            set
            {
                showimage.Text = value.ToString();
            }
        }
        public ScreenshotTimeEnum ScreenshotTime
        {
            get
            {
                return (ScreenshotTimeEnum)Enum.Parse(typeof(ScreenshotTimeEnum), screenshottime.Text);
            }
            set
            {
                screenshottime.Text = value.ToString();
            }
        }

        public bool SaveScreenshot
        {
            get
            {
                return savescreenshot.Text == "是";
            }
            set
            {
                savescreenshot.Text = value ? "是" : "否";
            }
        }
        public bool Shock
        {
            get
            {
                return shock.Text.Replace("震動：", "") == "開";
            }
            set
            {
                shock.Text = "震動：" + (value ? "開" : "關");
            }
        }

        public LookWayEnum LookWay
        {
            get
            {
                return (LookWayEnum)Enum.Parse(typeof(LookWayEnum), lookway.Text);
            }
            set
            {
                lookway.Text = value.ToString();
            }
        }

        public static ImageSource CopyImageSource(ImageSource Source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)Source;
            Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
            MemoryStream data = (MemoryStream)task.Result;
            byte[] image = data.ToArray();
            Stream stream = new MemoryStream(image);
            return ImageSource.FromStream(() => stream);
        }

        public static byte[] ImageSourceToBytes(ImageSource Source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)Source;
            Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
            MemoryStream data = (MemoryStream)task.Result;
            byte[] image = data.ToArray();
            return image;
        }

        public ShowTemplate (MainPage mainPage)
		{
			InitializeComponent ();

            this.mainPage = mainPage;

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            MagicImage.WidthRequest = 78;
            MagicImage.HeightRequest = (MagicImage.WidthRequest + 40) * ((mainPage.Bounds.Height - 50) / mainPage.Bounds.Width) - 40;
            BackGroundImage.WidthRequest = 78;
            BackGroundImage.HeightRequest = (BackGroundImage.WidthRequest + 40) * ((mainPage.Bounds.Height - 50) / mainPage.Bounds.Width) - 40; 
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            (mainPage.FindByName<Grid>("controler")).IsVisible = false;
            SettingTemplate settingTemplate = new SettingTemplate();
            settingTemplate.mainPage = mainPage;
            settingTemplate.showTemplate = this;
            Navigation.PushAsync(settingTemplate);
        }
        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            mainPage.DeleteShow = this;
            (mainPage.FindByName<Grid>("OnDelete")).IsVisible = true;
        }
    }
}