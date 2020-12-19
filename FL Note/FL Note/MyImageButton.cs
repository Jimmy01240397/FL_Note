using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchTracking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note
{
	public class MyImageButton : Frame
	{
        StackLayout stackLayout;
        Image image;

        public event EventHandler Clicked;

        public double ButtonHeight
        {
            get
            {
                return HeightRequest + 40;
            }
            set
            {
                HeightRequest = value - 40;
                //image.TranslationX = HeightRequest / 2;
            }
        }

        public double ButtonWidth
        {
            get
            {
                return base.WidthRequest + 40;
            }
            set
            {
                WidthRequest = value - 40;
                //image.TranslationY = WidthRequest / 2;
            }
        }

        public double ImageHeight
        {
            get
            {
                return image.HeightRequest;
            }
            set
            {
                image.HeightRequest = value;
            }
        }

        public double ImageWidth
        {
            get
            {
                return image.WidthRequest;
            }
            set
            {
                image.WidthRequest = value;
            }
        }

        public ImageSource Source
        {
            get
            {
                return image.Source;
            }
            set
            {
                image.Source = value;
            }
        }

        public MyImageButton ():base()
		{
            stackLayout = new StackLayout();
            Content = stackLayout;
            stackLayout.BackgroundColor = Color.Transparent;
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnClicked;
            stackLayout.GestureRecognizers.Add(tapGestureRecognizer);
            image = new Image();
            stackLayout.Children.Add(image);
            stackLayout.Margin = -20;
            image.HorizontalOptions = LayoutOptions.CenterAndExpand;
            image.VerticalOptions = LayoutOptions.CenterAndExpand;
            image.WidthRequest = stackLayout.WidthRequest;
            image.HeightRequest = stackLayout.HeightRequest;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Type type = sender.GetType();
            Clicked?.Invoke(this, e);
        }
    }
}