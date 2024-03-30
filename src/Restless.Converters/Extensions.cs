using System;

namespace Restless.Converters
{
    /// <summary>
    /// Provides public extension methods
    /// </summary>
    public static class Extensions
    {
        #region Other extensions
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