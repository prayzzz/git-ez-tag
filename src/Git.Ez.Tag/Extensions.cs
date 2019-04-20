using System.Text.RegularExpressions;

namespace Git.Ez.Tag
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
    }
}