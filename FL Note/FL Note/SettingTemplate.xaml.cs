using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingTemplate : ContentPage
	{
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
                }
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
            //this.ScaleTo(0);
            Navigation.PopAsync();
            MyTabbedPage tabbedPage = mainPage.FindByName<MyTabbedPage>("controler");
            tabbedPage.IsVisible = true;
            //tabbedPage.FindByName<CarouselView>("carouselView").ScaleTo(0);
            //tabbedPage.FindByName<CarouselView>("carouselView").ScaleTo(1);
            //tabbedPage.FindByName<CarouselView>("carouselView").ItemsSource = tabbedPage.view.ToArray();
        }
    }
}