namespace UnifaceLibrary
{
    public static class StringExtensions
    {
        /// <summary>
        /// Stored internally in Uniface with CR - transform to standard Windows CRLF so that VS code can correctly search.
        /// </summary>
        public static string TransformUnifaceLineEndings(this string text)
        {
            return text.Replace("\r", "\r\n");
        }
                
        public static string UnescapeXml(this string xmlInput)
        {
            return xmlInput
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'");
        }
    }
}
