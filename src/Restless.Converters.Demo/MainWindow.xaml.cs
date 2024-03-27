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
        private const string LoadFileName = @"D:\Development\Visual_Studio\Projects\Restless.Converters\.docs\site.full.html";
        private const string SaveFileName = @"D:\Development\Visual_Studio\Projects\Restless.Converters\.docs\rich.flow.xaml";
        private static readonly string DataFormat = DataFormats.XamlPackage;

        public MainWindow()
        {
            InitializeComponent();
            PasteHandler.Create(Rich);
            PasteHandler.Create(TextBoxHtml, new PasteHandlerOptions(HtmlPasteAction.ConvertToText));
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

        private void ButtonSaveRichTextClick(object sender, RoutedEventArgs e)
        {
            SaveRichTextBoxToFile(SaveFileName);
        }

        private void ButtonLoadRichTextClick(object sender, RoutedEventArgs e)
        {
            LoadRichTextBoxFromFile(SaveFileName);
        }

        private void LoadRichTextBoxFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    using (FileStream fileStream = new(fileName, FileMode.OpenOrCreate))
                    {
                        TextRange range = new(Rich.Document.ContentStart, Rich.Document.ContentEnd);
                        range.Load(fileStream, DataFormat);
                    }
                }
                catch (Exception)
                {
                    Rich.AppendText("File is not right format or is corrupted");
                }
            }
            else
            {
                Rich.AppendText("File not found");
            }
        }
        private void SaveRichTextBoxToFile(string fileName)
        {
            using (FileStream fileStream = new(fileName, FileMode.Create))
            {
                TextRange range = new(Rich.Document.ContentStart, Rich.Document.ContentEnd);
                range.Save(fileStream, DataFormat);
            }
        }


        private void ButtonLoadHtmlClick(object sender, RoutedEventArgs e)
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
                    IsOutputIndented = true,
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
            if (TextBoxXaml.Text.Length > 0)
            {
                TryLoadRichTextBox();
            }
        }

        [DebuggerHidden]
        private void TryLoadRichTextBox()
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
                Rich.AppendText(ex.Message);
            }
        }

        private void ButtonClearHtmlClick(object sender, RoutedEventArgs e)
        {
            TextBoxHtml.Clear();
        }
    }
}