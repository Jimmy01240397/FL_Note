using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Graphics.Drawable;
using Android.Views;
using Android.Widget;
using FL_Note;
using FL_Note.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AppCompToolbar = Android.Support.V7.Widget.Toolbar;

[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(CustomNavigationPageRenderer))]
namespace FL_Note.Droid
{
    public class CustomNavigationPageRenderer : Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer
    {
        public AppCompToolbar toolbar;
        public Activity context;

        public CustomNavigationPageRenderer(Context ctx) : base(ctx)
        {
            context = (Activity)ctx;
        }

        protected override Task<bool> OnPushAsync(Page view, bool animated)
        {
            var retVal = base.OnPushAsync(view, animated);

            /*toolbar = context.FindViewById<AppCompToolbar>(Droid.Resource.Id.toolbar);

            if (toolbar != null)
            {
                if(toolbar.NavigationIcon != null)
                {
                    //toolbar.NavigationIcon = Android.Support.V7.Content.Res.AppCompatResources.GetDrawable(context, Resource.Drawable.back);
                }
                //int cont = 0, cont2 = 0;
                for (var i = 0; i < toolbar.ChildCount; i++)
                {
                    var imageButton = toolbar.GetChildAt(i) as Android.Widget.ImageButton;

                    //if (imageButton != null) cont++;

                    var drawerArrow = imageButton?.Drawable as DrawerArrowDrawable;
                    if (drawerArrow == null)
                        continue;

                    if(toolbar.ChildCount == 3)
                    {
                        Android.Widget.TextView textView = new TextView(context);
                        Toolbar.LayoutParams l2 = new Toolbar.LayoutParams(Toolbar.LayoutParams.WrapContent, Toolbar.LayoutParams.WrapContent);
                        //l2.Height
                        text.setLayoutParams(l2);
                        textView.Text = "返回";
                        toolbar.AddView(textView);
                    }

                    //else
                    //    cont2++;

                    //imageButton.SetImageDrawable(Forms.Context.GetDrawable(Resource.Drawable.hamburger));
                }
            }*/

            return retVal;
        }
    }
}