using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using System.Collections.Generic;

namespace FL_Note.Droid
{
    [Activity(Label = "FL Note", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const int Permissions_REQUEST_LOCATION = 99;

        static Dictionary<int, Action> AfterRequest = new Dictionary<int, Action>();

        public static void CallPermissions(string[] permissions, Action DoAfterCall)
        {
            List<string> permissionslist = new List<string>();
            foreach(string per in permissions)
            {
                if (!CheckPermission(per))
                {
                    permissionslist.Add(per);
                }
            }
            if (permissionslist.Count != 0)
            {
                AfterRequest.Add(Permissions_REQUEST_LOCATION, DoAfterCall);
                ActivityCompat.RequestPermissions(Instance, permissionslist.ToArray(), Permissions_REQUEST_LOCATION);
            }
        }

        public static bool CheckPermission(string permissions)
        {
            return ContextCompat.CheckSelfPermission(Instance, permissions) == Permission.Granted;
        }

        internal static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Instance = this;
            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == Permissions_REQUEST_LOCATION)
            {
                AfterRequest[Permissions_REQUEST_LOCATION]?.Invoke();
                AfterRequest.Remove(Permissions_REQUEST_LOCATION);
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public override void OnBackPressed()
        {
            
        }
    }
}