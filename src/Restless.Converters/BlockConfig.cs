using System;
using System.Windows;
using System.Windows.Media;

namespace Restless.Converters
{
    /// <summary>
    /// Represents the configuration for a block element
    /// </summary>
    public class BlockConfig
    {
        #region Private
        private double fontSize;
        private double spacing;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the block config id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public double FontSize
        {
            get => fontSize;
            set => fontSize = Math.Clamp(value, 8.0, 100.0);
        }

        /// <summary>
        /// Gets or sets the font weight
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the text alignment
        /// </summary>
        public TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Gets or sets the background brush
        /// </summary>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush
        /// </summary>
        public Brush Foreground { get; set; }

        /// <summary>
        /// Gets or sets the border brush
        /// </summary>
        public Brush BorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the border thickness
        /// </summary>
        public Thickness BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the padding
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// Gets or sets the spacing.
        /// The default is double.NaN, which indicates the value won't be applied.
        /// </summary>
        /// <remarks>
        /// Only certain elements (such as table) use this value.
        /// When setting this property, the value is clamped between 0 and 50.
        /// </remarks>
        public double Spacing
        {
            get => spacing;
            set => spacing = Math.Clamp(value, 0, 50);
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockConfig"/> class
        /// </summary>
        /// <param name="id">The block id</param>
        public BlockConfig(string id) : this(id, 16.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockConfig"/> class
        /// </summary>
        /// <param name="id">The block id</param>
        /// <param name="fontSize">The font size</param>
        public BlockConfig(string id, double fontSize) : this(id, fontSize, FontWeights.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockConfig"/> class
        /// </summary>
        /// <param name="id">The block id</param>
        /// <param name="fontSize">The font size</param>
        /// <param name="fontWeight">The font weight</param>
        public BlockConfig(string id, double fontSize, FontWeight fontWeight)
        {
            ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));
            Id = id;
            FontSize = fontSize;
            FontWeight = fontWeight;
            TextAlignment = TextAlignment.Left;
            Spacing = double.NaN;
        }
        #endregion
    }
}