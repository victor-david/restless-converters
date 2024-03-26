using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Restless.Converters
{
    public class HtmlToXamlConverter
    {
        #region Private
        private readonly string html;
        private readonly ConverterOptions options;
        #endregion

        /************************************************************************/

        #region Public properties
        public BlockConfigCollection BlockConfig { get; }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="html">The html to convert</param>
        public HtmlToXamlConverter(string html) : this(html, new ConverterOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="html">The html to convert</param>
        /// <param name="options">Converter options</param>
        public HtmlToXamlConverter(string html, ConverterOptions options)
        {
            ArgumentException.ThrowIfNullOrEmpty(html);
            ArgumentNullException.ThrowIfNull(options);
            this.html = html;
            this.options = options;

            BlockConfig = new BlockConfigCollection(this.options.AddDefaultBlockConfigs);
            BlockConfig.Add(new BlockConfig("img", 18, FontWeights.DemiBold)
            {
                Background = Brushes.Yellow,
                Foreground = Brushes.DimGray,
                BorderBrush = Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
            });
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="html">The html to convert</param>
        /// <returns>A new instance of <see cref="HtmlToXamlConverter"/></returns>
        public static HtmlToXamlConverter Create(string html)
        {
            return new HtmlToXamlConverter(html);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="html">The html to convert</param>
        /// <param name="options">Converter options</param>
        /// <returns>A new instance of <see cref="HtmlToXamlConverter"/></returns>
        public static HtmlToXamlConverter Create(string html, ConverterOptions options)
        {
            return new HtmlToXamlConverter(html, options);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts the html specified in the constructor
        /// </summary>
        /// <returns>A XAML string</returns>
        public string Convert()
        {
            XmlDocument xamlDoc = new();
            HtmlDocument htmlDoc = new();

            XmlElement xamlTopElement = xamlDoc.AddSectionElement();
            xamlTopElement.ApplyBlockConfig(options.SectionConfig);

            htmlDoc.LoadHtml(html);
            htmlDoc.DocumentNode.RemoveAllCommentNodes();

            WalkNodes(htmlDoc.DocumentNode, xamlTopElement);

            if (options.SetPreserve)
            {
                xamlTopElement.SetAttribute("xml:space", "preserve");
            }

            XmlWriterSettings xmlWriterSettings = new()
            {
                Indent = options.IsOutputIndented,
                OmitXmlDeclaration = true,
            };

            StringBuilder builder = new();
            xamlDoc.Save(XmlWriter.Create(builder, xmlWriterSettings));
            return builder.ToString();
        }
        #endregion

        /************************************************************************/

        #region HtmlNodeType (Ref)
        //public enum HtmlNodeType
        //{
        //    // The root of a document.
        //    Document,
        //    // An HTML element.
        //    Element,
        //    // An HTML comment.
        //    Comment,
        //    // A text node is always the child of an element or a document node.
        //    Text
        //}
        #endregion

        /************************************************************************/

        #region Private methods (Main)
        private void WalkNodes(HtmlNode parentNode, XmlElement xamlElement)
        {
            foreach (HtmlNode node in parentNode.ChildNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    ProcessElementNode(node, xamlElement);
                }

                if (node.NodeType == HtmlNodeType.Text)
                {
                    ProcessTextNode(node, xamlElement);
                }
            }
        }

        private void ProcessElementNode(HtmlNode node, XmlElement parent)
        {
            switch (node.GetHtmlElementType())
            {
                case HtmlElementType.Section:
                    ProcessSectionElement(node, parent);
                    break;
                case HtmlElementType.Paragraph:
                    ProcessParagraphElement(node, parent);
                    break;
                case HtmlElementType.Inline:
                    ProcessInlineElement(node, parent);
                    break;
                case HtmlElementType.List:
                    ProcessListElement(node, parent);
                    break;
                case HtmlElementType.ListItem:
                    ProcessListItemElement(node, parent);
                    break;
                case HtmlElementType.Table:
                    ProcessTableElement(node, parent);
                    break;
                case HtmlElementType.Image:
                    ProcessImageElement(node, parent);
                    break;
                case HtmlElementType.Unknown:
                    ProcessUnknownElement(node, parent);
                    break;

            }
        }

        private void ProcessTextNode(HtmlNode node, XmlElement parent)
        {
            if (!node.IsEmptyText())
            {
                if (parent.IsNamed(Tokens.XamlParagraph))
                {
                    parent.AddChildText(node.GetCleanInnerText(false));
                }

                if (parent.IsNamed(Tokens.XamlSection))
                {
                    ProcessParagraphElement(node, parent);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Section, paragraph, inline)
        private void ProcessSectionElement(HtmlNode node, XmlElement parent)
        {
            if (node.HasOnlyText())
            {
                XmlElement para = parent.AddParagraphElement();
                ApplyBlockConfig(node, para);
                para.AddChildText(node.GetCleanInnerText(true));
            }
            else
            {
                XmlElement section = parent.AddSectionElement();
                section.ApplyBlockConfig(options.SectionConfig);
                WalkNodes(node, section);
            }

        }

        private void ProcessParagraphElement(HtmlNode node, XmlElement parent)
        {
            if (parent.IsNamed(Tokens.XamlSection))
            {
                XmlElement paragraph = parent.AddParagraphElement();
                ApplyBlockConfig(node, paragraph);
                if (node.HasOnlyText())
                {
                    paragraph.AddChildText(node.GetCleanInnerText(true));
                }
                else
                {
                    WalkNodes(node, paragraph);
                }
            }

            if (parent.IsNamed(Tokens.XamlParagraph))
            {
                ProcessInlineElement(node, parent);
            }
        }

        private static void ProcessInlineElement(HtmlNode node, XmlElement parent)
        {
            if (parent.IsNamed(Tokens.XamlParagraph))
            {
                XmlElement run = parent.AddRunElement();
                run.AddChildText(node.GetCleanInnerText(false));

                switch (node.Name)
                {
                    case Tokens.HtmlBold:
                    case Tokens.HtmlStrong:
                        run.SetAttribute(Tokens.XamlFontWeight, Tokens.XamlFontWeightBold);
                        break;
                    case Tokens.HtmlItalic:
                    case Tokens.HtmlEmphasis:
                        run.SetAttribute(Tokens.XamlFontStyle, Tokens.XamlFontStyleItalic);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (List)
        private void ProcessListElement(HtmlNode node, XmlElement parent)
        {
            if (parent.IsNamed(Tokens.XamlSection))
            {
                XmlElement list = parent.AddListElement().AddListMarkerStyle(node.Name);
                ApplyBlockConfig(node, list);
                WalkNodes(node, list);
            }
        }

        private void ProcessListItemElement(HtmlNode node, XmlElement parent)
        {
            if (parent.IsNamed(Tokens.XamlList))
            {
                XmlElement listItemParagraph = parent.AddListItemElement().AddParagraphElement();
                WalkNodes(node, listItemParagraph);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Table)
        private void ProcessTableElement(HtmlNode node, XmlElement parent)
        {
            if (node.Name == Tokens.HtmlTable && parent.IsNamed(Tokens.XamlSection))
            {
                ProcessTable(node, parent);
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/how-to-define-a-table-with-xaml?view=netframeworkdesktop-4.8
        /// </summary>
        private void ProcessTable(HtmlNode node, XmlElement parent)
        {
            int colCount = 0;
            int rowCount = 0;
            foreach (HtmlNode row in node.Descendants(Tokens.HtmlTableRow))
            {
                rowCount++;
                colCount = Math.Max(colCount, row.Descendants(Tokens.HtmlTableHeadCell).Count());
                colCount = Math.Max(colCount, row.Descendants(Tokens.HtmlTableCell).Count());
            }

            if (colCount > 0)
            {
                XmlElement table = parent.AddTableElement();
                ApplyBlockConfig(node, table);

                XmlElement columnGroup = table.AddTableColumnGroupElement();
                for (int k = 0; k < colCount; k++)
                {
                    columnGroup.AddTableColumnElement();
                }

                XmlElement tableRowGroup = table.AddTableRowGroupElement();

                foreach (HtmlNode rowNode in node.Descendants(Tokens.HtmlTableRow))
                {
                    XmlElement tableRow = tableRowGroup.AddTableRowElement();
                    foreach (HtmlNode dNode in rowNode.Descendants())
                    {
                        if (dNode.Name == Tokens.HtmlTableHeadCell || dNode.Name == Tokens.HtmlTableCell)
                        {
                            XmlElement cell = tableRow.AddTableCellElement();
                            ApplyBlockConfig(dNode, cell);
                            cell.SetColumnSpan(dNode, colCount);
                            cell.SetRowSpan(dNode, rowCount);
                            cell.AddParagraphElement().AddChildText(dNode.GetCleanInnerText(true));

                        }
                    }
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (image)
        private void ProcessImageElement(HtmlNode node, XmlElement element)
        {
            if (node.Attributes["src"] is HtmlAttribute attrib)
            {
                if (element.IsNamed(Tokens.XamlParagraph))
                {
                    element.AddRunElement().AddChildText(attrib.Value);
                }

                if (element.IsNamed(Tokens.XamlSection))
                {
                    XmlElement paragraph = element.AddParagraphElement();
                    ApplyBlockConfig(node, paragraph);
                    paragraph.AddRunElement().AddChildText(attrib.Value);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ProcessUnknownElement(HtmlNode node, XmlElement parent)
        {
            if (options.ProcessUnknown)
            {
                if (parent.IsNamed(Tokens.XamlParagraph))
                {
                    XmlElement span = parent.AddSpanElement().SetForeground(Brushes.Red);
                    span.AddRunElement().AddChildText("[Unknown node ");
                    span.AddRunElement().SetBold().AddChildText(node.Name);
                    span.AddRunElement().AddChildText(", inner text: ");
                    span.AddRunElement().SetBold().AddChildText(node.InnerText);
                    span.AddRunElement().AddChildText("]");
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (other)
        private void ApplyBlockConfig(HtmlNode node, XmlElement element)
        {
            if (BlockConfig.Get(node.Name) is BlockConfig blockConfig)
            {
                element.ApplyBlockConfig(blockConfig);
            }
        }
        #endregion
    }
}