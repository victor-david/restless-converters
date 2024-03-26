using System;

namespace Restless.Converters
{
    internal class HtmlPasteItem
    {
        private const string VersionId = "Version:";
        private const string SourceUrlId = "SourceURL:";
        private const string StartFragmentId = "<!--StartFragment-->";
        private const string EndFragmentId = "<!--EndFragment-->";

        public string Input { get; }
        public string Version { get; }
        public string SourceUrl { get; }
        public string Fragment { get; }
        public bool HasFragment => !string.IsNullOrEmpty(Fragment);

        internal HtmlPasteItem(string input)
        {
            Input = input ?? string.Empty;

            string[] lines = Input.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                if (line.StartsWith(VersionId, StringComparison.InvariantCultureIgnoreCase))
                {
                    Version = line[VersionId.Length..];
                }

                if (line.StartsWith(SourceUrlId, StringComparison.InvariantCultureIgnoreCase))
                {
                    SourceUrl = line[SourceUrlId.Length..];
                }

                if (line.StartsWith(StartFragmentId, StringComparison.InvariantCultureIgnoreCase) &&
                    line.EndsWith(EndFragmentId, StringComparison.InvariantCultureIgnoreCase))
                {
                    Fragment = line.Substring(StartFragmentId.Length, line.Length - StartFragmentId.Length - EndFragmentId.Length);
                }
            }
        }

        public override string ToString()
        {
            return $"{Version}{Environment.NewLine}{SourceUrl}{Environment.NewLine}{Fragment}";
        }
    }
}