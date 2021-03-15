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
        SettingTemplate _settingTemplate;
        public SettingTemplate settingTemplate
        {
            get
            {
                return _settingTemplate;
            }
            set
            {
                _settingTemplate = value;

                drawing.AllViewSize = new SKSize(value.mainPage.FindByName<DrawLayout>("drawlayout").BackImage.Width, value.mainPage.FindByName<DrawLayout>("drawlayout").BackImage.Height);
                drawing.ViewImage = SKImage.FromEncodedData(((MyImageSource)value.Image).ImageBytes);
                drawing.BackImage = SKImage.FromEncodedData(((MyImageSource)value.BackImage).ImageBytes);
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
            settingTemplate.Image = MyImageSource.FromBytes(drawing.GetImageByte(SKEncodedImageFormat.Png, DrawLayout.ChooseImage.View));
            //drawing.Save(SKEncodedImageFormat.Png, Elements.DrawLayout.ChooseImage.View, "FL-Note", Guid.NewGuid().ToString() + ".png");
        }

        private void Drawing_EndDraw(object sender, EventArgs e)
        {
            restore.IsEnabled = drawing.CanRestore;
        }
    }
}