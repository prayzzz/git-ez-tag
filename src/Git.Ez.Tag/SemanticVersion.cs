using System;
using System.Text.RegularExpressions;

namespace Git.Ez.Tag
{
    public enum SemanticVersionElement
    {
        None = 0,
        Major,
        Minor,
        Patch
    }

    /// <summary>
    ///     Supports only Major, Minor and Patch element.
    /// </summary>
    public class SemanticVersion
    {
        private static readonly Regex ParseEx = new Regex(@"^(?<major>\d+)(\.(?<minor>\d+))?(\.(?<patch>\d+))?$",
                                                          RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        public SemanticVersion(int major, int? minor, int? patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; }

        public int? Minor { get; }

        public int? Patch { get; }

        public static bool TryParse(string version, out SemanticVersion semanticVersion)
        {
            var match = ParseEx.Match(version);
            if (!match.Success)
            {
                semanticVersion = null;
                return false;
            }

            var major = int.Parse(match.Groups["major"].Value);
            var minor = match.Groups["minor"].ToIntOrNull();
            var patch = match.Groups["patch"].ToIntOrNull();

            semanticVersion = new SemanticVersion(major, minor, patch);
            return true;
        }

        public SemanticVersion Increase(SemanticVersionElement semanticVersionElement)
        {
            switch (semanticVersionElement)
            {
                case SemanticVersionElement.None:
                    return this;
                case SemanticVersionElement.Major:
                    return new SemanticVersion(
                        Major + 1,
                        Minor.HasValue ? (int?) 0 : null,
                        Patch.HasValue ? (int?) 0 : null
                    );
                case SemanticVersionElement.Minor:
                    if (Minor.HasValue)
                    {
                        return new SemanticVersion(
                            Major,
                            Minor + 1,
                            Patch.HasValue ? (int?) 0 : null
                        );
                    }
                    else
                    {
                        throw new ArgumentException($"Can't increase version {semanticVersionElement}");
                    }
                case SemanticVersionElement.Patch:
                    if (Patch.HasValue)
                    {
                        return new SemanticVersion(
                            Major,
                            Minor,
                            Patch + 1
                        );
                    }
                    else
                    {
                        throw new ArgumentException($"Can't increase version {semanticVersionElement}");
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(semanticVersionElement), semanticVersionElement, null);
            }
        }

        public override string ToString()
        {
            var version = Major.ToString();

            if (Minor.HasValue)
            {
                version += $".{Minor}";

                if (Patch.HasValue)
                {
                    version += $".{Patch}";
                }
            }

            return version;
        }
    }
}