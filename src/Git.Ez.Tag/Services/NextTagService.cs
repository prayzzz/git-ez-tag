using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Git.Ez.Tag.Services
{
    public class NextTagService
    {
        private const string TagPrompt = "> What's your next Tag?";
        private readonly Git _git;

        private readonly ILogger<NextTagService> _logger;

        public NextTagService(ILogger<NextTagService> logger, Git git)
        {
            _logger = logger;
            _git = git;
        }

        public string GetNextTag(DirectoryInfo repository, SemVerElement semVerElement)
        {
            var latestTag = _git.GetLatestTag(repository);
            if (latestTag == null)
            {
                _logger.LogDebug("No Tag found.");
                return Prompt.GetString(TagPrompt, "1.0.0");
            }

            _logger.LogInformation($"Latest Tag is '{latestTag}'");

            if (SemanticVersion.TryParse(latestTag, out var semver))
            {
                switch (semVerElement)
                {
                    case SemVerElement.None:
                        var next = semver.Minor.HasValue
                                       ? semver.Increase(SemVerElement.Minor).ToString()
                                       : semver.Increase(SemVerElement.Major).ToString();

                        return Prompt.GetString(TagPrompt, next);
                    case SemVerElement.Major:
                    case SemVerElement.Minor:
                    case SemVerElement.Patch:
                        return semver.Increase(semVerElement).ToString();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(semVerElement), semVerElement, null);
                }
            }

            return Prompt.GetString(TagPrompt);
        }
    }
}