using FL_Note.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        void OnClearClicked(object sender, EventArgs e)
        {
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

                            ShowTemplate showTemplate1 = new ShowTemplate(this);
                            showTemplate1.HorizontalOptions = LayoutOptions.FillAndExpand;
                            showTemplate1.HeightRequest = 330;
                            ShowPages.Children.Add(showTemplate1);

                            ShowTemplate showTemplate2 = new ShowTemplate(this);
                            showTemplate2.HorizontalOptions = LayoutOptions.FillAndExpand;
                            showTemplate2.HeightRequest = 330;
                            ShowPages.Children.Add(showTemplate2);

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
            ShowPages.Children.Clear();
        }
    }
}
