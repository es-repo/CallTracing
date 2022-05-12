namespace CallTracing.Utilities
{
    public static partial class StringExtentions
    {
        public static string TruncateWithEllipsis(
            this string str,
            int maxLength,
            bool useSingleUnicodeCharForEllipsis = false)
        {
            if (str.Length <= maxLength)
            {
                return str;
            }

            string ellipsis = useSingleUnicodeCharForEllipsis ? "…" : "...";

            var lenth = maxLength - ellipsis.Length >= 0 ? maxLength - ellipsis.Length : 0;

            return str.Substring(0, lenth) + ellipsis;
        }
    }
}
