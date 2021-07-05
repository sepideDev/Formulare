using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;

using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.IO;
using Java.IO;
using Xamarin.Forms;
using Android.Graphics;
using Formulare.Droid;
using Android.Content.PM;
using Xamarin.Essentials;
using Android.Webkit;
using Android.Graphics.Drawables;
using Plugin.CurrentActivity;

//using Android.Content.Context.GetExternalFilesDir;

[assembly: Dependency(typeof(Formulare.Droid.SaveAndroid))]
namespace Formulare.Droid
{
    public class SaveAndroid : ISaveWindowsPhone
    {
        private Activity mContext;

        [Obsolete]
        public void Save(string fileName, String contentType, MemoryStream s)
        {
            string root = null;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
                root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //root = "/interner Speicher/Download/";

            Java.IO.File myDir = new Java.IO.File(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, fileName);

            if (file.Exists()) file.Delete();

            try
            {
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(s.ToArray());

                outs.Flush();
                outs.Close();

            }
            catch (Exception e)
            {

            }
            if (file.Exists())
            {
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, mimeType);
                Android.App.Application.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            }
        }

        public static Activity activity { get; set; }
        public object R { get; private set; }

        [Obsolete]
        public System.Threading.Tasks.Task<byte[]> CaptureAsync()
        {
            //// This code works fine as long as what I need from the xaml fits in the screen without scrolling
            //var activity = Forms.Context as Activity;

            //var view = activity.Window.DecorView;
            //view.DrawingCacheEnabled = true;

            //Bitmap bitmap = view.GetDrawingCache(true);

            //byte[] bitmapData;

            //using (var stream = new MemoryStream())
            //{
            //    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
            //    bitmapData = stream.ToArray();
            //}



            //// But i want my all element in a ScrollView to be save in my pdf. I tried out the code in the Xamarin forums link but was not able to get it to work https://forums.xamarin.com/discussion/146465/how-to-convert-the-all-content-in-scroll-view-to-bitmap-in-xamarin-forum

            ////            scrollversion
            var activity = Forms.Context as Activity;

            //var activity = CrossCurrentActivity.Current.Activity as MainActivity;
            ViewGroup content = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
            ViewGroup frameLayout = (ViewGroup)content.GetChildAt(0);
            ViewGroup relativeLayout = (ViewGroup)frameLayout.GetChildAt(0);
            ViewGroup platFormRenderer = (ViewGroup)relativeLayout.GetChildAt(0);
            ViewGroup myNavigationPageRenderer = (ViewGroup)platFormRenderer.GetChildAt(0);
            ViewGroup pageRenderer = (ViewGroup)myNavigationPageRenderer.GetChildAt(0);
            ViewGroup scrollViewRenderer = (ViewGroup)pageRenderer.GetChildAt(0);// --> scrollViewRenderer



            //Adjust this to find your own scrollViewRenderer
            Android.Views.View z = scrollViewRenderer as Android.Widget.ScrollView;
            z.SetBackgroundColor(Android.Graphics.Color.White);
            int totalHeight = z.Height;
            int totalWidth = z.Width;

            Bitmap bitmap = getBitmapFromView(z, (int)totalHeight, (int)totalWidth);

            byte[] bitmapData;

            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                bitmapData = stream.ToArray();
            }


            return Task.FromResult(bitmapData);
        }

        public Bitmap getBitmapFromView(Android.Views.View view, int totalHeight, int totalWidth)
        {
            Bitmap returnedBitmap = Bitmap.CreateBitmap(totalWidth, totalHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(returnedBitmap);
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                bgDrawable.Draw(canvas);
            else
                canvas.DrawColor(Android.Graphics.Color.White);
            canvas.Save();
            view.Draw(canvas);
            canvas.Restore();
            return returnedBitmap;
        }



        // Package : Plugin.Permissions
        [Obsolete]
        private async Task CheckAppPermissions_01()
        {
            if ((int)Android.OS.Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                try
                {
                    var status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync<Plugin.Permissions.StoragePermission>();
                    if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        if (await Plugin.Permissions.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
                        {
                            //Gunna need that location
                        }

                        status = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionAsync<Plugin.Permissions.StoragePermission>();
                    }

                    if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        //Query permission
                    }
                    else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                    {
                        //location denied
                    }
                }
                catch (Exception ex)
                {
                    //Something went wrong
                }
            }
        }


        //Package : Xamarin.Essentials
        private async Task CheckAppPermissions_02()
        {
            if ((int)Android.OS.Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                try
                {
                    var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                    if (status != Xamarin.Essentials.PermissionStatus.Granted)
                    {
                        status = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                    }

                    if (status == Xamarin.Essentials.PermissionStatus.Granted)
                    {
                        //Query permission
                    }
                    else if (status != Xamarin.Essentials.PermissionStatus.Unknown)
                    {
                        //location denied
                    }
                }
                catch (Exception ex)
                {
                    //Something went wrong
                }
            }
        }




        async Task ISaveWindowsPhone.Save(string filename, string contentType, MemoryStream stream)
        {

            //Package : Plugin.Permissions
            //await CheckAppPermissions_01();

            //Package : Xamarin.Essentials
            await CheckAppPermissions_02();

            string root = null;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
                root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            

            Java.IO.File myDir = new Java.IO.File(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, filename);

            if (file.Exists()) 
                file.Delete();

            try
            {
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(stream.ToArray());

                outs.Flush();
                outs.Close();

            }
            catch (Exception e)
            {

            }
            if (file.Exists())
            {
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, mimeType);
                Android.App.Application.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            }
        }
    }
}