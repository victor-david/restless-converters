using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Restless.Converters.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LoadFileName = @"D:\Development\Visual_Studio\Projects\Restless.Converters\.docs\site.partial.html";
        private const string SaveFileName = @"D:\Development\Visual_Studio\Projects\Restless.Converters\.docs\rich.flow.xaml";

        public MainWindow()
        {
            InitializeComponent();
            PasteHandler.Create(Rich);
            PasteHandler.Create(TextBoxHtml);
            AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(OnNavigationRequest));
        }

        private void OnNavigationRequest(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Hyperlink source && source.NavigateUri.ToString().IsValidUri())
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = source.NavigateUri.ToString(),
                    UseShellExecute = true,
                });
            }
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            SaveContentToFile(SaveFileName);
        }

        private void SaveContentToFile(string fileName)
        {
            using (FileStream fileStream = new(fileName, FileMode.Create))
            {
                TextRange range = new(Rich.Document.ContentStart, Rich.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Xaml);
            }
        }


        private void ButtonLLoadClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(LoadFileName))
            {
                TextBoxHtml.Text = File.ReadAllText(LoadFileName);
            }
        }

        private void ButtonConvertClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxHtml.Text.Length > 0)
            {
                TextBoxXaml.Text = HtmlToXamlConverter.Create(new ConverterOptions()
                {
                    ProcessUnknown = true
                }).SetBlockConfig(new BlockConfig("img")
                {
                    Background = Brushes.BlanchedAlmond,
                    Foreground = Brushes.DimGray,
                    BorderBrush = Brushes.DarkBlue,
                    BorderThickness = new Thickness(2),
                    Padding = new Thickness(10),
                }).SetHtml(TextBoxHtml.Text).Convert();
            }
        }

        [DebuggerHidden]
        private void ButtonToRichClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MemoryStream mem = new(Encoding.UTF8.GetBytes(TextBoxXaml.Text)))
                {
                    TextRange range = new(Rich.Document.ContentStart, Rich.Document.ContentEnd);
                    range.Load(mem, DataFormats.Xaml);
                }
            }
            catch (Exception ex)
            {
                using (MemoryStream mem = new(Encoding.UTF8.GetBytes(ex.Message)))
                {
                    TextRange range = new(Rich.Document.ContentStart, Rich.Document.ContentEnd);
                    range.Load(mem, DataFormats.Rtf);
                }
            }
        }

        private void ButtonClearHtmlClick(object sender, RoutedEventArgs e)
        {
            TextBoxHtml.Clear();
        }
    }
}