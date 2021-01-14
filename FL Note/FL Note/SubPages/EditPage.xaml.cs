using FL_Note.Elements;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouchTracking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.SubPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPage : ContentPage
	{
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

                drawing.ViewImage = SKImage.FromEncodedData(ShowTemplate.ImageSourceToBytes(value.Image));
                drawing.BackImage = SKImage.FromEncodedData(ShowTemplate.ImageSourceToBytes(value.BackgroundImage));
            }
        }

        public EditPage ()
		{
			InitializeComponent ();
		}

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            drawing.Clear();
        }

        private void OnRestoreClicked(object sender, EventArgs e)
        {
            drawing.Restore();
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            drawing.Save(SKEncodedImageFormat.Png, Elements.DrawLayout.ChooseImage.View, "FL-Note", Guid.NewGuid().ToString() + ".png");
        }

        private void Drawing_EndDraw(object sender, EventArgs e)
        {
            restore.IsEnabled = drawing.CanRestore;
        }
    }
}