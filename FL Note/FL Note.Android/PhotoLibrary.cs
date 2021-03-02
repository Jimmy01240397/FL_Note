using System.Threading.Tasks;

using Android.Content;
using Android.Media;
using Android.OS;
using Java.IO;

using Xamarin.Forms;

using FL_Note.Droid;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using System;
using Android.Support.Design.Widget;
using Environment = Android.OS.Environment;
using FL_Note.Interface;
using System.Numerics;
using Android.Content.Res;

namespace FL_Note.Droid
{
    public class PhotoLibrary : IPhotoLibrary
    {
        public void OpenGallery(Action<byte[]> DoAfterCall)
        {
            MainActivity.OpenGallery(DoAfterCall);
        }

        public void CropPhoto(byte[] data, Vector2 Aspect, Vector2 Output, Action<byte[]> DoAfterCall)
        {
            MainActivity.CropPhoto(data, Aspect, Output, DoAfterCall);
        }

        // Saving photos requires android.permission.WRITE_EXTERNAL_STORAGE in AndroidManifest.xml

        public async Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            async Task<bool> doing()
            {
                try
                {
                    File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
                    File folderDirectory = picturesDirectory;

                    if (!string.IsNullOrEmpty(folder))
                    {
                        folderDirectory = new File(picturesDirectory, folder);
                        folderDirectory.Mkdirs();
                    }

                    using (File bitmapFile = new File(folderDirectory, filename))
                    {
                        bitmapFile.CreateNewFile();

                        using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                        {
                            await outputStream.WriteAsync(data);
                        }

                        // Make sure it shows up in the Photos gallery promptly.
                        MediaScannerConnection.ScanFile(MainActivity.Instance,
                                                        new string[] { bitmapFile.Path },
                                                        new string[] { "image/png", "image/jpeg" }, null);
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }
            if (!MainActivity.CheckPermission(Manifest.Permission.WriteExternalStorage))
            {
                MainActivity.CallPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, async (permissions, grantResults) => 
                {
                    bool check = true;
                    foreach(Permission permission in grantResults)
                    {
                        check = check && permission == Permission.Granted;
                    }
                    if (check)
                    {
                        await doing();
                    }
                });
                return true;
            }
            else
            {
                return await doing();
            }
        }

        public bool isApplicationInTheBackground()
        {
            bool isInBackground = false;

            Android.App.ActivityManager.RunningAppProcessInfo myProcess = new Android.App.ActivityManager.RunningAppProcessInfo();
            Android.App.ActivityManager.GetMyMemoryState(myProcess);
            isInBackground = myProcess.Importance != Android.App.Importance.Foreground;

            return isInBackground;
        }
    }
}