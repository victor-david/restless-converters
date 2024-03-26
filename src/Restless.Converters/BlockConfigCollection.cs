using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;

namespace Restless.Converters
{
    /// <summary>
    /// Represents a collection of <see cref="BlockConfig"/> objects
    /// </summary>
    public class BlockConfigCollection
    {
        #region Private
        private readonly List<BlockConfig> storage;
        #endregion

        /************************************************************************/

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockConfigCollection"/> class
        /// </summary>
        public BlockConfigCollection() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockConfigCollection"/> class
        /// </summary>
        /// <param name="addDefaults">true to add default block configurations</param>
        public BlockConfigCollection(bool addDefaults)
        {
            storage = new List<BlockConfig>();
            if (addDefaults)
            {
                AddDefaults();
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds or updates a block config
        /// </summary>
        /// <param name="blockConfig">The block config to add</param>
        /// <remarks>
        /// If a block config with the block id doesn't yet exist, it will be added.
        /// If it does exist, the values of the existing config will be updated.
        /// </remarks>
        public void Add(BlockConfig blockConfig)
        {
            ArgumentNullException.ThrowIfNull(nameof(blockConfig));
            if (!Contains(blockConfig.Id))
            {
                storage.Add(blockConfig);
            }
            else
            {
                UpdateBlockConfig(blockConfig);
            }
        }

        public bool Contains(string blockId) => Get(blockId) is not null;

        public BlockConfig Get(string blockId) => storage.FirstOrDefault(b => b.Id == blockId);

        public void Clear()
        {
            storage.Clear();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void UpdateBlockConfig(BlockConfig source)
        {
            if (Get(source.Id) is BlockConfig dest)
            {
                dest.Background = source.Background;
                dest.BorderBrush = source.BorderBrush;
                dest.BorderThickness = source.BorderThickness;
                dest.FontSize = source.FontSize;
                dest.FontWeight = source.FontWeight;
                dest.Foreground = source.Foreground;
                dest.Padding = source.Padding;
            }
        }

        private void AddDefaults()
        {
            Add(new BlockConfig("h1", 24, FontWeights.Bold)
            {
                Foreground = Brushes.Red,
                BorderBrush = Brushes.DimGray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 0, 0, 1)
            });

            Add(new BlockConfig("h2", 20, FontWeights.Bold)
            {
                Foreground = Brushes.Green
            });
            Add(new BlockConfig("h3", 18, FontWeights.Bold));
            Add(new BlockConfig("h4", 14, FontWeights.Bold));

            Add(new BlockConfig(Tokens.HtmlUnorderedList, 24)
            {
                BorderBrush = Brushes.Green,
                BorderThickness = new Thickness(3),
                Padding = new Thickness(50, 3, 3, 3)

            });

            Add(new BlockConfig(Tokens.HtmlOrderedList, 24)
            {
                BorderBrush = Brushes.Blue,
                BorderThickness = new Thickness(3),
                Padding = new Thickness(50, 3, 3, 3)
            });

            Add(new BlockConfig(Tokens.HtmlTable)
            {
                Spacing = 8,
            });

            Add(new BlockConfig(Tokens.HtmlTableHeadCell)
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Background = Brushes.LightSteelBlue,
                Padding = new Thickness(5)
            });

            Add(new BlockConfig(Tokens.HtmlTableCell)
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5)
            });
        }
        #endregion
    }
}