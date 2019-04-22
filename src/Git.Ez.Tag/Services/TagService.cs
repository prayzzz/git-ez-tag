using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Git.Ez.Tag.Services
{
    public class TagService
    {
        private const string TagPrompt = "> What's your next Tag?";
        private readonly Git _git;

        private readonly ILogger<TagService> _logger;

        public TagService(ILogger<TagService> logger, Git git)
        {
            _logger = logger;
            _git = git;
        }

        public string GetAndIncrementTag(DirectoryInfo repository, SemanticVersionElement semanticVersionElement)
        {
            var latestTag = _git.GetLatestTag(repository);
            if (latestTag == null)
            {
                _logger.LogInformation("Repository doesn't contain any Tags.");
                return Prompt.GetString(TagPrompt, "1.0.0");
            }

            _logger.LogInformation($"Latest Tag is '{latestTag}'");

            if (SemanticVersion.TryParse(latestTag, out var semver))
            {
                switch (semanticVersionElement)
                {
                    case SemanticVersionElement.None:
                        var next = semver.Minor.HasValue
                                       ? semver.Increase(SemanticVersionElement.Minor).ToString()
                                       : semver.Increase(SemanticVersionElement.Major).ToString();

                        return Prompt.GetString(TagPrompt, next);
                    case SemanticVersionElement.Major:
                    case SemanticVersionElement.Minor:
                    case SemanticVersionElement.Patch:
                        return semver.Increase(semanticVersionElement).ToString();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(semanticVersionElement), semanticVersionElement, null);
                }
            }

            return Prompt.GetString(TagPrompt);
        }
    }
}