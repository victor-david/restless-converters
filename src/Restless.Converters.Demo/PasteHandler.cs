using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Restless.Converters.Demo
{
    internal static class PasteHandler
    {
        #region Private
        private static double MaxImageDimension = DefaultMaxImagePasteSize;

        private static class OtherFormats
        {
            public const string SysDrawingBitmap = "System.Drawing.Bitmap";
            public const string Format17 = "Format17";
            public const string DeviceIndependentBitmap = "DeviceIndependentBitmap";
        }
        #endregion

        /************************************************************************/

        #region Constants
        internal const double MinMaxImagePasteSize = 100;
        internal const double MaxMaxImagePasteSize = 800;
        internal const double DefaultMaxImagePasteSize = 500;
        #endregion

        /************************************************************************/

        #region Internal methods
        internal static void Initialize(DependencyObject element)
        {
            DataObject.AddPastingHandler(element, OnPaste);
        }

        internal static void SetMaxImageDimension(double maxImageDimension)
        {
            MaxImageDimension = Math.Clamp(maxImageDimension, MinMaxImagePasteSize, MaxMaxImagePasteSize);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            //ExamineDataObject(e.DataObject);
            DataObject dataObject = (DataObject)e.DataObject;
            TryHandleImageData(dataObject, e);
            TryHandleHtmlData(dataObject, e);
        }

        private static void TryHandleImageData(DataObject dataObject, DataObjectPastingEventArgs e)
        {
            if (dataObject.ContainsImage() && dataObject.GetImage() is BitmapSource bitmapSource)
            {
                double largestDimension = Math.Max(bitmapSource.Width, bitmapSource.Height);
                if (largestDimension > MaxImageDimension)
                {
                    double scale = MaxImageDimension / largestDimension;
                    if (ToBitmap(bitmapSource) is Bitmap bitmap)
                    {
                        Bitmap resizedBitmap = new(bitmap, new System.Drawing.Size((int)(bitmap.Width * scale), (int)(bitmap.Height * scale)));
                        if (ToBitmapSource(resizedBitmap) is BitmapSource resizedBitmapSource)
                        {
                            DataObject newDataObject = new();
                            newDataObject.SetImage(resizedBitmapSource);
                            e.DataObject = newDataObject;
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private static void TryHandleHtmlData(DataObject dataObject, DataObjectPastingEventArgs e)
        {
            if (!e.Handled && dataObject.ContainsHtml() && dataObject.GetData(DataFormats.Html) is string html)
            {
                HtmlPasteItem item = new(html);
                if (item.HasFragment)
                {
                    DataObject obj = new();
                    obj.SetText(item.Fragment);
                    e.DataObject = obj;
                }

                e.Handled = true;
            }
        }

        private static bool ContainsHtml(this DataObject data)
        {
            return data.GetDataPresent(DataFormats.Html);
        }
        #endregion

        /************************************************************************/

        #region Private methods (debug helper)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static void ExamineDataObject(IDataObject dataObject)
        {
            DataObject data = (DataObject)dataObject;
            Debug.WriteLine($"Contains html: {data.ContainsHtml()}");
            Debug.WriteLine($"Contains image: {data.ContainsImage()}");
            Debug.WriteLine($"Contains text: {data.ContainsText()}");
            Debug.WriteLine($"Contains audio: {data.ContainsAudio()}");
            Debug.WriteLine($"Contains file drop list: {data.ContainsFileDropList()}");

            foreach (string format in data.GetFormats())
            {
                try
                {
                    Debug.WriteLine(format);
                    Debug.WriteLine("--------Start");
                    var payload = dataObject.GetData(format);
                    Debug.WriteLine(payload);
                    Debug.WriteLine("---------End");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Bitmap conversion)
        // https://stackoverflow.com/questions/2284353/is-there-a-good-way-to-convert-between-bitmapsource-and-bitmap
        private static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          bitmap.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        private static Bitmap ToBitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
        #endregion
    }
}