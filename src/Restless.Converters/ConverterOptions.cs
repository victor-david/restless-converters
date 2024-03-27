namespace Restless.Converters
{
    /// <summary>
    /// Represents converter options that affect <see cref="HtmlToXamlConverter"/>
    /// </summary>
    public class ConverterOptions
    {
        #region Properties
        public const string SectionBlockId = "section";

        /// <summary>
        /// Gets the block config to apply to sections
        /// </summary>
        public BlockConfig SectionConfig { get; }

        /// <summary>
        /// Gets or sets a value that determines whether default block configs
        /// are added to the block config collection. The default is true.
        /// </summary>
        public bool AddDefaultBlockConfigs { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if unknown nodes are processed.
        /// The default value is false.
        /// </summary>
        public bool ProcessUnknown { get; set; }

        /// <summary>
        /// Gets or sets a value that determeines if "xml:space", "preserve" is added
        /// The default value is false.
        /// </summary>
        public bool SetPreserve { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether output is indented.
        /// The default is true.
        /// </summary>
        public bool IsOutputIndented { get; set; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterOptions"/> class
        /// </summary>
        public ConverterOptions()
        {
            SetPreserve = false;
            IsOutputIndented = false;
            AddDefaultBlockConfigs = true;
            ProcessUnknown = false;
            SectionConfig = new BlockConfig(SectionBlockId, 12.5);
        }
        #endregion
    }
}