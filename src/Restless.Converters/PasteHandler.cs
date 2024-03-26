using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Restless.Converters
{
    public class PasteHandler
    {
        #region Private
        private double maxImageDimenison;
        private static class OtherFormats
        {
            public const string SysDrawingBitmap = "System.Drawing.Bitmap";
            public const string Format17 = "Format17";
            public const string DeviceIndependentBitmap = "DeviceIndependentBitmap";
        }
        #endregion

        /************************************************************************/

        #region Constants (public)
        /// <summary>
        /// Gets the minimum value that may be assigned to <see cref="MaxImageDimension"/>.
        /// </summary>
        public const double MinMaxImagePasteSize = 100;

        /// <summary>
        /// Gets the maximum value that may be assigned to <see cref="MaxImageDimension"/>.
        /// </summary>
        public const double MaxMaxImagePasteSize = 800;

        /// <summary>
        /// Gets the default value for <see cref="MaxImageDimension"/>.
        /// </summary>
        public const double DefaultMaxImagePasteSize = 500;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the maximum image dimension.
        /// </summary>
        /// <remarks>
        /// When pasting an image, if it has a dimension greater than this value, it will be resized.
        /// This property is clamped between <see cref="MinMaxImagePasteSize"/> and <see cref="MaxMaxImagePasteSize"/>.
        /// The default value for this property is <see cref="DefaultMaxImagePasteSize"/>.
        /// </remarks>
        public double MaxImageDimension
        {
            get => maxImageDimenison;
            set => maxImageDimenison = Math.Clamp(value, MinMaxImagePasteSize, MaxMaxImagePasteSize);
        }

        /// <summary>
        /// Gets the <see cref="HtmlToXamlConverter"/> used to convert incoming html
        /// </summary>
        public HtmlToXamlConverter Converter
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="PasteHandler"/>
        /// </summary>
        /// <param name="element">The element to handle.</param>
        /// <returns></returns>
        public static PasteHandler Create(DependencyObject element)
        {
            return new PasteHandler(element);
        }

        private PasteHandler(DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element, nameof(element));
            DataObject.AddPastingHandler(element, OnPaste);
            MaxImageDimension = DefaultMaxImagePasteSize;
            Converter = HtmlToXamlConverter.Create();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            //ExamineDataObject(e.DataObject);
            DataObject dataObject = (DataObject)e.DataObject;
            TryHandleHtmlData(sender, dataObject, e);
            TryHandleImageData(dataObject, e);
        }

        private void TryHandleImageData(DataObject dataObject, DataObjectPastingEventArgs e)
        {
            if (!e.Handled && dataObject.ContainsImage() && dataObject.GetImage() is BitmapSource bitmapSource)
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

        private void TryHandleHtmlData(object sender, DataObject dataObject, DataObjectPastingEventArgs e)
        {
            if (!e.Handled && dataObject.GetHtml() is string html)
            {
                HtmlPasteItem item = new(html);
                if (item.HasFragment)
                {
                    DataObject obj = new();

                    if (sender is RichTextBox)
                    {
                        string xaml = Converter.SetHtml(item.Fragment).Convert();
                        obj.SetData(DataFormats.Xaml, xaml);
                    }

                    if (sender is TextBox)
                    {
                        obj.SetText(item.Fragment);
                    }

                    e.DataObject = obj;
                    e.Handled = true;
                }
            }
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

    internal static class Helper
    {
        internal static bool ContainsHtml(this DataObject data) => data.GetDataPresent(DataFormats.Html);
        internal static string GetHtml(this DataObject data) => data.ContainsHtml() ? data.GetData(DataFormats.Html) as string : null;
    }
}