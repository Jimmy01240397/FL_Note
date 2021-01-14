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

[assembly: Dependency(typeof(PhotoLibrary))]

namespace FL_Note.Droid
{
    public class PhotoLibrary : IPhotoLibrary
    {
        public Task<System.IO.Stream> PickPhotoAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<System.IO.Stream>();

            // Return Task object
            return MainActivity.Instance.PickImageTaskCompletionSource.Task;
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
                MainActivity.CallPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, async () => { await doing(); });
                return true;
            }
            else
            {
                return await doing();
            }
        }
    }
}