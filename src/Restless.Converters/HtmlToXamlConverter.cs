﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml;

namespace Restless.Converters
{
    public class HtmlToXamlConverter
    {
        #region Private
        private string siteBase;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the html to be converted.
        /// </summary>
        public string Html
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the converter options.
        /// </summary>
        public ConverterOptions Options { get; }

        /// <summary>
        /// Gets the block config collection.
        /// </summary>
        public BlockConfigCollection BlockConfig { get; }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Creates a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <returns>A new instance of <see cref="HtmlToXamlConverter"/></returns>
        public static HtmlToXamlConverter Create()
        {
            return new HtmlToXamlConverter(new ConverterOptions());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="options">Converter options</param>
        /// <returns>A new instance of <see cref="HtmlToXamlConverter"/></returns>
        public static HtmlToXamlConverter Create(ConverterOptions options)
        {
            return new HtmlToXamlConverter(options);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToXamlConverter"/> class
        /// </summary>
        /// <param name="html">The html to convert</param>
        /// <param name="options">Converter options</param>
        private HtmlToXamlConverter(ConverterOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            Options = options;
            BlockConfig = new BlockConfigCollection(Options.AddDefaultBlockConfigs);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the html to be converted
        /// </summary>
        /// <param name="html">The html</param>
        /// <returns>This instance</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="html"/> is null or empty
        /// </exception>
        public HtmlToXamlConverter SetHtml(string html)
        {
            ArgumentException.ThrowIfNullOrEmpty(html, nameof(html));
            Html = html;
            return this;
        }

        /// <summary>
        /// Adds (or updates) a <see cref="BlockConfig"/> object to the collection
        /// </summary>
        /// <param name="blockConfig">The block config</param>
        /// <returns>This instance</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blockConfig"/> is null
        /// </exception>
        /// <remarks>
        /// You can also add and update block config objects via the collection property directly.
        /// This method provides a way to chain calls to the <see cref="HtmlToXamlConverter"/> instance.
        /// </remarks>
        public HtmlToXamlConverter SetBlockConfig(BlockConfig blockConfig)
        {
            ArgumentNullException.ThrowIfNull(blockConfig);
            BlockConfig.Add(blockConfig);
            return this;
        }

        /// <summary>
        /// Converts the html specified in the constructor
        /// </summary>
        /// <returns>A XAML string</returns>
        /// <exception cref="ArgumentException">
        /// The <see cref="Html"/> property has not been set.
        /// </exception>
        public string Convert()
        {
            ArgumentException.ThrowIfNullOrEmpty(Html, nameof(Html));

            XmlDocument xamlDoc = new();
            HtmlDocument htmlDoc = new();

            XmlElement xamlTopElement = xamlDoc.AddSectionElement();
            xamlTopElement.ApplyBlockConfig(Options.SectionConfig);

            htmlDoc.LoadHtml(Html);
            htmlDoc.DocumentNode.RemoveAllCommentNodes();

            // Get the site base if it exists. May be used later to complete relative images
            siteBase = GetSiteBase(htmlDoc.DocumentNode.SelectSingleNode("//head/base"));

            HtmlNode startNode = htmlDoc.DocumentNode.SelectSingleNode("//body") ?? htmlDoc.DocumentNode;

            WalkNodes(startNode, xamlTopElement);

            if (Options.SetPreserve)
            {
                xamlTopElement.SetAttribute("xml:space", "preserve");
            }

            XmlWriterSettings xmlWriterSettings = new()
            {
                Indent = Options.IsOutputIndented,
                OmitXmlDeclaration = true,
            };

            StringBuilder builder = new();
            xamlDoc.Save(XmlWriter.Create(builder, xmlWriterSettings));
            return builder.ToString();
        }
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
                section.ApplyBlockConfig(Options.SectionConfig);
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
                switch (node.Name)
                {
                    case Tokens.HtmlAnchor:
                        parent.AddHyperlinkElement().SetNavigateUri(node).AddChildText(node.GetCleanInnerText(true));
                        break;
                    case Tokens.HtmlBold:
                    case Tokens.HtmlStrong:
                        parent.AddRunElement().SetBold().AddChildText(node.GetCleanInnerText(false));
                        break;
                    case Tokens.HtmlItalic:
                    case Tokens.HtmlEmphasis:
                        parent.AddRunElement().SetItalic().AddChildText(node.GetCleanInnerText(false));
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
        private void ProcessImageElement(HtmlNode node, XmlElement parent)
        {
            if (node.Attributes[Tokens.HtmlSource] is HtmlAttribute attrib)
            {
                string imgSource = GetCompleteImageSource(attrib.Value);

                if (parent.IsNamed(Tokens.XamlParagraph))
                {
                    parent.AddImageElement().SetSource(imgSource);
                }

                if (parent.IsNamed(Tokens.XamlSection))
                {
                    XmlElement paragraph = parent.AddParagraphElement();
                    ApplyBlockConfig(node, paragraph);
                    paragraph.AddImageElement().SetSource(imgSource);
                }
            }
        }

        private string GetCompleteImageSource(string imgSource)
        {
            if (!imgSource.IsValidUri() && !string.IsNullOrEmpty(siteBase))
            {
                if (imgSource.StartsWith("/"))
                {
                    imgSource = imgSource[1..];
                }
                // siteBase always ends with a slash. See GetSiteBase() below
                return $"{siteBase}{imgSource}";
            }
            return imgSource;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ProcessUnknownElement(HtmlNode node, XmlElement parent)
        {
            if (Options.ProcessUnknown)
            {
                if (parent.IsNamed(Tokens.XamlParagraph))
                {
                    AddUnknownToParagraph(node, parent);
                }

                if (parent.IsNamed(Tokens.XamlSection))
                {
                    AddUnknownToParagraph(node, parent.AddParagraphElement());
                }
            }
        }

        private static void AddUnknownToParagraph(HtmlNode node, XmlElement parent)
        {
            XmlElement span = parent.AddSpanElement().SetForeground(Brushes.Red);
            span.AddRunElement().AddChildText("[Unknown node ");
            span.AddRunElement().SetBold().AddChildText(node.Name);
            span.AddRunElement().AddChildText(", inner text: ");
            span.AddRunElement().AddChildText(node.InnerText);
            span.AddRunElement().AddChildText("]");
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

        private static string GetSiteBase(HtmlNode baseNode)
        {
            if (baseNode != null && baseNode.Attributes[Tokens.HtmlHref] is HtmlAttribute attrib)
            {
                string baseStr = attrib.Value;
                if (!baseStr.EndsWith("/"))
                {
                    baseStr += "/";
                }
                return baseStr;
            }
            return null;
        }
        #endregion
    }
}