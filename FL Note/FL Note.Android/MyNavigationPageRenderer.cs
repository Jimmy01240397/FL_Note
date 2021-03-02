/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FL_Note.Droid;
using FL_Note.Elements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace FL_Note.Droid
{
    public class MyNavigationPageRenderer: NavigationPageRenderer
    {
        public MyNavigationPageRenderer(Context ctx) : base(ctx)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);

            var height = 0;

            Resources resources = Context.Resources;
            int resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                height = resources.GetDimensionPixelSize(resourceId);
            }
            MyNavigationPage.BarHeight = height;
        }
    }
}*/