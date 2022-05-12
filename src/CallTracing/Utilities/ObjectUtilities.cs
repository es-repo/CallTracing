using System.Collections;

namespace CallTracing.Utilities
{
    public static class ObjectUtilities
    {
        public static string ObjectToString(object? o, int maxLength = 2000)
        {
            static string GetFullString(object? o)
            {
                if (o == null)
                {
                    return "null";
                }

                return o is IEnumerable enumerable
                        ? "[" + string.Join(", ", enumerable.Cast<object>().Select(o => o?.ToString() ?? "null")) + "]"
                        : o?.ToString() ?? "null";
            }

            var fullString = GetFullString(o);

            return fullString.TruncateWithEllipsis(maxLength);
        }
    }
}
