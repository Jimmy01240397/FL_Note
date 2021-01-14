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
                    for(int i = 0; i < (backGrounds.Length - 1) / 2 + 1; i++)
                    {
                        ImageButton imageButton = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.Start
                        };
                    }
                }
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

                ((Image)((Grid)WorkingImage.Content).Children[0]).Source = ShowTemplate.CopyImageSource(value.BackgroundImage);
                ((Image)((Grid)WorkingImage.Content).Children[1]).Source = ShowTemplate.CopyImageSource(value.Image);                
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
            Navigation.PopAsync();
            MyTabbedPage tabbedPage = mainPage.FindByName<MyTabbedPage>("controler");
            tabbedPage.IsVisible = true;
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            EditPage editPage = new EditPage();
            editPage.showTemplate = showTemplate;
            Navigation.PushAsync(editPage);
        }

        private void PictureButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}