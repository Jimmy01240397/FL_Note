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
using Android.Graphics;
using Android.Net;
using Uri = Android.Net.Uri;
using System.Numerics;
using Android;

namespace FL_Note.Droid
{
    [Activity(Label = "FL Note", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public enum Funccode
        {
            None = -1,
            AskPermission,
            Open_Gallery,
            Crop_Photo
        }

        static Dictionary<int, Action<object>> AfterRequest = new Dictionary<int, Action<object>>();

        internal static MainActivity Instance { get; private set; }

        public static void CallPermissions(string[] permissions, Action<string[], Permission[]> DoAfterCall)
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
                AfterRequest.Add((int)Funccode.AskPermission, (data) =>
                {
                    object[] outdata = (object[])data;
                    DoAfterCall?.Invoke((string[])outdata[0], (Permission[])outdata[1]);
                });
                ActivityCompat.RequestPermissions(Instance, permissionslist.ToArray(), (int)Funccode.AskPermission);
            }
        }

        public static bool CheckPermission(string permissions)
        {
            return ContextCompat.CheckSelfPermission(Instance, permissions) == Permission.Granted;
        }

        public static void OpenGallery(Action<byte[]> DoAfterCall)
        {
            Intent intent = new Intent();
            //intent.SetType("image/*");
            intent.SetDataAndType(Android.Provider.MediaStore.Images.Media.ExternalContentUri, "image/*");
            intent.SetAction(Intent.ActionPick);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            AfterRequest.Add((int)Funccode.Open_Gallery, (data) =>
            {
                DoAfterCall?.Invoke((byte[])data);
            });

            Instance.StartActivityForResult(intent, (int)Funccode.Open_Gallery);
        }

        public static void CropPhoto(byte[] data, Vector2 Aspect, Vector2 Output, Action<byte[]> DoAfterCall)
        {
            Uri tempUri = null;
            FileStream tempStream = null;

            string tempname = Guid.NewGuid().ToString();
            Java.IO.File path = CreateFile(tempname + ".png");
            tempStream = File.OpenWrite(path.Path);
            tempStream.Write(data, 0, data.Length);
            tempStream.Close();
            tempUri = GetUri(path);

            string tempname2 = Guid.NewGuid().ToString();
            Uri nowstate = Uri.FromFile(CreateFile(tempname2 + ".png"));

            Intent intent = new Intent("com.android.camera.action.CROP");
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
            intent.SetDataAndType(tempUri, "image/*");
            intent.PutExtra("crop", "true");
            // aspectX aspectY 是寬高的比例
            intent.PutExtra("aspectX", (int)Aspect.X);
            intent.PutExtra("aspectY", (int)Aspect.Y);
            // outputX outputY 是裁剪圖片寬高
            intent.PutExtra("outputX", (int)Output.X);
            intent.PutExtra("outputY", (int)Output.Y);
            intent.PutExtra("scale", true);
            intent.PutExtra("return-data", false);
            intent.PutExtra("noFaceDetection", true);
            intent.PutExtra(Android.Provider.MediaStore.ExtraOutput, nowstate);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            AfterRequest.Add((int)Funccode.Crop_Photo, (data2) =>
            {
                MemoryStream stream = new MemoryStream();
                BitmapFactory.DecodeStream(Instance.ContentResolver.OpenInputStream(nowstate)).Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();
                Java.IO.File tempFile = new Java.IO.File(tempUri.Path);
                Java.IO.File tempFile2 = new Java.IO.File(nowstate.Path);
                tempFile.Delete();
                path.Delete();
                tempFile2.Delete();
                DoAfterCall?.Invoke(stream.ToArray());
            });

            Instance.StartActivityForResult(intent, (int)Funccode.Crop_Photo);
        }

        private static Java.IO.File CreateFile(string name)
        {
            Java.IO.File file = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory, name);
            if (file.Exists())
            {
                file.Delete();
            }
            file.CreateNewFile();

            return file;
        }

        private static Uri GetUri(Java.IO.File file)
        {
            Uri uri;
            if (Build.VERSION.SdkInt >= (BuildVersionCodes)24)
            {
                uri = FileProvider.GetUriForFile(Instance, Instance.PackageName + ".fileprovider", file);
                //uri = Uri.FromFile(file);
            }
            else
            {
                uri = Uri.FromFile(file);
            }

            return uri;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Instance = this;
            App.photoLibrary = new PhotoLibrary();
            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
            Window window = Window;
            window.AddFlags(WindowManagerFlags.KeepScreenOn);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CallPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, null);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == (int)Funccode.AskPermission)
            {
                AfterRequest[requestCode]?.Invoke(new object[] { permissions, grantResults });
                AfterRequest.Remove(requestCode);
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if ((resultCode == Result.Ok) && (intent != null))
            {
                switch (requestCode)
                {
                    case (int)Funccode.Open_Gallery:
                        {
                            Uri uri = intent.Data;
                            MemoryStream stream = new MemoryStream();
                            BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(uri)).Compress(Bitmap.CompressFormat.Png, 100, stream);
                            stream.Close();
                            AfterRequest[requestCode]?.Invoke(stream.ToArray());
                            AfterRequest.Remove(requestCode);
                        }
                        break;
                    case (int)Funccode.Crop_Photo:
                        {
                            AfterRequest[requestCode]?.Invoke(null);
                            AfterRequest.Remove(requestCode);
                        }
                        break;
                }
            }
            else
            {
                if (AfterRequest.ContainsKey(requestCode))
                {
                    AfterRequest.Remove(requestCode);
                }
            }
        }

        public override void OnBackPressed()
        {
            
        }
    }
}