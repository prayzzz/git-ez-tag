using System.IO;
using System.Text.RegularExpressions;

namespace GitEzTag
{
    public static class Extensions
    {
        /// <exception cref="System.FormatException">Value is not in the correct format.</exception>
        public static int? ToIntOrNull(this Group group)
        {
            if (group.Success)
            {
                return int.Parse(group.Value);
            }

            return null;
        }

        public static string GetFirstLine(this string str)
        {
            return new StringReader(str).ReadLine();
        }
    }
}