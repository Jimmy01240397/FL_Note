using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowTemplate : StackLayout
	{
        MainPage mainPage;
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
            (mainPage.FindByName<MyTabbedPage>("controler")).IsVisible = false;
            SettingTemplate settingTemplate = new SettingTemplate();
            settingTemplate.mainPage = mainPage;
            Navigation.PushAsync(settingTemplate);
        }
    }
}