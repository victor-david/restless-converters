namespace Restless.Converters
{
    internal static class Tokens
    {
        internal static readonly string[] SectionBlocks = { "main", "div", "section", "header", "figure", "figcaption", "footer" };
        internal static readonly string[] ParagraphBlocks = { "h1", "h2", "h3", "h4", "h5", "h6", "p" };
        internal static readonly string[] InlineElements = { "b", "strong", "i", "em", "span", "a" };
        internal static readonly string[] ListElements = { "ul", "ol" };
        internal static readonly string[] ListItemElements = { "li" };
        internal static readonly string[] TableElements = { "table", "thead", "tbody", "tfooter", "tr", "th", "td" };
        internal static readonly string[] IgnoredElements = { "head", "meta", "link", "script", "nav", "button" };

        internal static readonly string[] ImageElements = { "img" };

        internal static readonly string[] AcceptsSection = { XamlFlowDocument, XamlSection };
        internal static readonly string[] AcceptsParagraph = { XamlSection, XamlListItem };
        internal static readonly string[] AcceptsList = { XamlSection };
        internal static readonly string[] AcceptsListItem = { XamlList };
        internal static readonly string[] AcceptsInline = { XamlParagraph, XamlSpan, XamlRun, XamlBold, XamlItalic };
        internal static readonly string[] AcceptsText = { XamlParagraph, XamlSpan, XamlRun, XamlBold, XamlItalic };

        internal const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal const string XamlFlowDocument = "FlowDocument";
        internal const string XamlSection = "Section";
        internal const string XamlParagraph = "Paragraph";
        internal const string XamlSpan = "Span";
        internal const string XamlRun = "Run";
        internal const string XamlBold = "Bold";
        internal const string XamlItalic = "Italic";
        internal const string XamlHyperlink = "Hyperlink";

        internal const string XamlImage = "Image";
        internal const string XamlSource = "Source";
        internal const string XamlStretch = "Stretch";
        internal const string XamlStretchNone = "None";

        internal const string XamlList = "List";
        internal const string XamlListItem = "ListItem";
        internal const string XamlListMarkerStyle = "MarkerStyle";
        internal const string XamlListMarkerStyleNone = "None";
        internal const string XamlListMarkerStyleDecimal = "Decimal";
        internal const string XamlListMarkerStyleDisc = "Disc";
        internal const string XamlListMarkerStyleCircle = "Circle";
        internal const string XamlListMarkerStyleSquare = "Square";
        internal const string XamlListMarkerStyleBox = "Box";

        internal const string XamlTable = "Table";
        internal const string XamlTableCellSpacing = "CellSpacing";
        internal const string XamlTableColumnGroup = "Table.Columns";
        internal const string XamlTableColumn = "TableColumn";
        internal const string XamlTableRowGroup = "TableRowGroup";
        internal const string XamlTableRow = "TableRow";
        internal const string XamlTableCell = "TableCell";
        internal const string XamlTableCellColumnSpan = "ColumnSpan";
        internal const string XamlTableCellRowSpan = "RowSpan";

        internal const string XamlHeight = "Height";
        internal const string XamlWidth = "Width";
        internal const string XamlFontSize = "FontSize";
        internal const string XamlFontStyle = "FontStyle";
        internal const string XamlFontStyleItalic = "Italic";
        internal const string XamlFontWeight = "FontWeight";
        internal const string XamlFontWeightBold = "Bold";
        internal const string XamlBackground = "Background";
        internal const string XamlForeground = "Foreground";
        internal const string XamlBorderBrush = "BorderBrush";
        internal const string XamlBorderThickness = "BorderThickness";
        internal const string XamlPadding = "Padding";
        internal const string XamlNavigateUri = "NavigateUri";

        internal const string HtmlDiv = "div";
        internal const string HtmlBold = "b";
        internal const string HtmlStrong = "strong";
        internal const string HtmlItalic = "i";
        internal const string HtmlEmphasis = "em";
        internal const string HtmlSpan = "span";
        internal const string HtmlAnchor = "a";
        internal const string HtmlHref = "href";
        internal const string HtmlSource = "src";
        internal const string HtmlOrderedList = "ol";
        internal const string HtmlUnorderedList = "ul";
        internal const string HtmlListItem = "li";
        internal const string HtmlTable = "table";
        internal const string HtmlTableHead = "thead";
        internal const string HtmlTableBody = "tbody";
        internal const string HtmlTableFooter = "tfooter";
        internal const string HtmlTableRow = "tr";
        internal const string HtmlTableHeadCell = "th";
        internal const string HtmlTableCell = "td";
        internal const string HtmlTableColSpan = "colspan";
        internal const string HtmlTableRowSpan = "rowspan";
    }
}