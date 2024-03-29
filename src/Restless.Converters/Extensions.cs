using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Restless.Converters
{
    public static class Extensions
    {
        #region HtmlNode extensions

        public static HtmlElementType GetHtmlElementType(this HtmlNode node)
        {
            if (Tokens.SectionBlocks.Contains(node.Name))
            {
                return HtmlElementType.Section;
            }
            if (Tokens.ParagraphBlocks.Contains(node.Name))
            {
                return HtmlElementType.Paragraph;
            }
            if (Tokens.InlineElements.Contains(node.Name))
            {
                return HtmlElementType.Inline;
            }
            if (Tokens.ListElements.Contains(node.Name))
            {
                return HtmlElementType.List;
            }
            if (Tokens.ListItemElements.Contains(node.Name))
            {
                return HtmlElementType.ListItem;
            }
            if (Tokens.TableElements.Contains(node.Name))
            {
                return HtmlElementType.Table;
            }
            if (Tokens.ImageElements.Contains(node.Name))
            {
                return HtmlElementType.Image;
            }

            if (Tokens.IgnoredElements.Contains(node.Name))
            {
                return HtmlElementType.Ignore;
            }
            return HtmlElementType.Unknown;
        }

        public static bool HasZeroChildren(this HtmlNode node) => node.ChildNodes.Count == 0;

        /// <summary>
        /// Gets a boolean value that indicates if the node has any children to process.
        /// To qualify, the node must be type Element or type Text (and if type Text, not empty text)
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>true if children to process; otherwise, false</returns>
        public static bool HasChildrenToProcess(this HtmlNode node) =>
            node.ChildNodes.Where(c => c.NodeType == HtmlNodeType.Element || (c.NodeType == HtmlNodeType.Text && !c.IsEmptyText())).Any();

        /// <summary>
        /// Gets a boolean that indicates if this node contains only text node(s)
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns></returns>
        public static bool HasOnlyText(this HtmlNode node) => node.ChildCount(HtmlNodeType.Text) == node.ChildNodes.Count;

        /// <summary>
        /// Gets the count of children of the specified type
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="nodeType">The node type to get the count for</param>
        /// <returns>The count</returns>
        public static int ChildCount(this HtmlNode node, HtmlNodeType nodeType) => node.ChildNodes.Count(n => n.NodeType == nodeType);

        public static bool IsEmptyText(this HtmlNode node) => node.NodeType == HtmlNodeType.Text && node.InnerText.Trim().Length == 0;

        public static string GetCleanInnerText(this HtmlNode node) => node.InnerText.Replace("&nbsp;", " ");
        public static string GetCleanDirectInnerText(this HtmlNode node) => node.GetDirectInnerText().Replace("&nbsp;", " ");

        /// <summary>
        /// Removes all comment nodes from the parent and all its descendants
        /// </summary>
        /// <param name="parentNode">The parent nod</param>
        public static void RemoveAllCommentNodes(this HtmlNode parentNode)
        {
            List<HtmlNode> commentNodes = new();
            parentNode.PopulateCommentNodes(commentNodes);
            commentNodes.ForEach(node => node.Remove());
        }

        private static void PopulateCommentNodes(this HtmlNode parentNode, List<HtmlNode> commentNodes)
        {
            foreach (HtmlNode node in parentNode.ChildNodes)
            {
                if (node.NodeType == HtmlNodeType.Comment)
                {
                    commentNodes.Add(node);
                }
                node.PopulateCommentNodes(commentNodes);
            }
        }
        #endregion

        /************************************************************************/

        #region XamlElement extensions
        public static XmlElement AddSectionElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlSection);
        public static XmlElement AddParagraphElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlParagraph);
        public static XmlElement AddSpanElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlSpan);
        public static XmlElement AddRunElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlRun);
        public static XmlElement AddBoldElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlBold);
        public static XmlElement AddItalicElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlItalic);


        public static XmlElement AddHyperlinkElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlHyperlink);
        public static XmlElement AddImageElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlImage);

        public static XmlElement AddListElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlList);
        public static XmlElement AddListItemElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlListItem);
        public static XmlElement AddTableElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTable);

        public static XmlElement AddTableColumnGroupElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTableColumnGroup);
        public static XmlElement AddTableColumnElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTableColumn);
        public static XmlElement AddTableRowGroupElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTableRowGroup);
        public static XmlElement AddTableRowElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTableRow);
        public static XmlElement AddTableCellElement(this XmlNode parent) => AddChildElement(parent, Tokens.XamlTableCell);

        public static XmlElement AddListMarkerStyle(this XmlElement parent, string htmlName)
        {
            string attribValue =
                htmlName == Tokens.HtmlOrderedList ? Tokens.XamlListMarkerStyleDecimal :
                htmlName == Tokens.HtmlUnorderedList ? Tokens.XamlListMarkerStyleBox : Tokens.XamlListMarkerStyleNone;

            parent.SetAttribute(Tokens.XamlListMarkerStyle, attribValue);

            return parent;
        }

        public static XmlElement SetHeight(this XmlElement parent, double height)
        {
            parent.SetAttribute(Tokens.XamlHeight, Math.Max(0, height).ToString());
            return parent;
        }

        public static XmlElement SetForeground(this XmlElement parent, Brush brush)
        {
            if (brush is not null)
            {
                parent.SetAttribute(Tokens.XamlForeground, brush.ToString());
            }
            return parent;
        }

        public static XmlElement SetBold(this XmlElement parent)
        {
            parent.SetAttribute(Tokens.XamlFontWeight, Tokens.XamlFontWeightBold);
            return parent;
        }

        public static XmlElement SetItalic(this XmlElement parent)
        {
            parent.SetAttribute(Tokens.XamlFontStyle, Tokens.XamlFontStyleItalic);
            return parent;
        }

        public static XmlElement SetSource(this XmlElement parent, string source)
        {
            if (source.IsValidUri())
            {
                parent.SetAttribute(Tokens.XamlSource, source);
            }
            return parent;
        }

        public static XmlElement SetStretch(this XmlElement parent, Stretch stretch)
        {
            parent.SetAttribute(Tokens.XamlStretch, stretch.ToString());
            return parent;
        }

        public static XmlElement SetNavigateUri(this XmlElement parent, HtmlNode node)
        {
            if (node.Attributes[Tokens.HtmlHref] is HtmlAttribute attrib && !attrib.Value.StartsWith("#") && attrib.Value.IsValidUri())
            {
                parent.SetAttribute(Tokens.XamlNavigateUri, attrib.Value);
            }
            return parent;
        }

        private static XmlElement AddChildElement(this XmlNode parent, string elementName)
        {
            XmlElement childElement = GetCreator(parent).CreateElement
                (
                    qualifiedName: elementName, Tokens.XamlNamespace
                );
            parent.AppendChild(childElement);
            return childElement;
        }

        public static XmlElement AddChildText(this XmlElement parent, string text)
        {
            XmlText child = parent.GetCreator().CreateTextNode(text);
            parent.AppendChild(child);
            return parent;
        }

        public static void SetColumnSpan(this XmlElement cell, HtmlNode node, int maxColSpan)
        {
            if (cell.IsNamed(Tokens.XamlTableCell) && node.Attributes[Tokens.HtmlTableColSpan] is HtmlAttribute attrib)
            {
                if (int.TryParse(attrib.Value, out int result))
                {
                    result = Math.Clamp(result, 0, maxColSpan);
                    cell.SetAttribute(Tokens.XamlTableCellColumnSpan, result.ToString());
                }
            }
        }

        public static void SetRowSpan(this XmlElement cell, HtmlNode node, int maxRowSpan)
        {
            if (cell.IsNamed(Tokens.XamlTableCell) && node.Attributes[Tokens.HtmlTableRowSpan] is HtmlAttribute attrib)
            {
                if (int.TryParse(attrib.Value, out int result))
                {
                    result = Math.Clamp(result, 0, maxRowSpan);
                    cell.SetAttribute(Tokens.XamlTableCellRowSpan, result.ToString());
                }
            }
        }

        public static void ApplyBlockConfig(this XmlElement element, BlockConfig blockConfig)
        {
            element.SetAttribute(Tokens.XamlFontSize, blockConfig.FontSize.ToString());
            element.SetAttribute(Tokens.XamlFontWeight, blockConfig.FontWeight.ToString());
            if (blockConfig.Background is not null)
            {
                element.SetAttribute(Tokens.XamlBackground, blockConfig.Background.ToString());
            }
            if (blockConfig.Foreground is not null)
            {
                element.SetAttribute(Tokens.XamlForeground, blockConfig.Foreground.ToString());
            }

            if (blockConfig.BorderBrush != null)
            {
                element.SetAttribute(Tokens.XamlBorderBrush, blockConfig.BorderBrush.ToString());
            }

            if (!blockConfig.BorderThickness.IsZero())
            {
                element.SetAttribute(Tokens.XamlBorderThickness, blockConfig.BorderThickness.ToString());
            }

            if (!blockConfig.Padding.IsZero())
            {
                element.SetAttribute(Tokens.XamlPadding, blockConfig.Padding.ToString());
            }

            if (element.IsNamed(Tokens.XamlTable) && !double.IsNaN(blockConfig.Spacing))
            {
                element.SetAttribute(Tokens.XamlTableCellSpacing, blockConfig.Spacing.ToString());
            }
        }

        private static XmlDocument GetCreator(this XmlNode node) => node.NodeType == XmlNodeType.Document ? node as XmlDocument : node.OwnerDocument;


        public static bool IsNamed(this XmlNode node, string name) => node.Name == name;

        public static bool AcceptsParagraph(this XmlNode node) => Tokens.AcceptsParagraph.Contains(node.Name);
        public static bool AcceptsInline(this XmlNode node) => Tokens.AcceptsInline.Contains(node.Name);
        #endregion

        /************************************************************************/

        #region Other extensions
        public static bool IsZero(this Thickness t) => t.Bottom == 0 && t.Left == 0 && t.Right == 0 && t.Top == 0;

        public static bool IsValidUri(this string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                return false;
            }
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri temp))
            {
                return false;
            }
            return temp.Scheme == Uri.UriSchemeHttp || temp.Scheme == Uri.UriSchemeHttps;
        }
        #endregion
    }
}