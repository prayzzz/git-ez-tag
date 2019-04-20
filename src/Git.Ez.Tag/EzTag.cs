using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Git.Ez.Tag.Services;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Git.Ez.Tag
{
    public enum SemVerElement
    {
        None = 0,
        Major,
        Minor,
        Patch
    }

    internal class EzTag
    {
        private const string InitialRepositoryPath = ".";
        private readonly AnnotationService _annotationService;
        private readonly Git _git;

        private readonly ILogger<EzTag> _logger;
        private readonly NextTagService _nextTagService;

        public EzTag(ILogger<EzTag> logger, Git git, NextTagService nextTagService, AnnotationService annotationService)
        {
            _logger = logger;
            _git = git;
            _nextTagService = nextTagService;
            _annotationService = annotationService;
        }

        [Option("-p|--push", "Executes 'git push --tags' after creating the Tag", CommandOptionType.NoValue)]
        public bool IsAutoPush { get; set; }

        [Option("-i|--increase", "Auto increase the given part of the version", CommandOptionType.SingleValue, ValueName = "Major|Minor|Patch")]
        public SemVerElement SemVerElement { get; set; }

        [Option("-l|--lightweight", "Skip Tag annotation", CommandOptionType.NoValue)]
        public bool IsLightWeight { get; set; }

        private Task OnExecuteAsync()
        {
            var banner = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Git.Ez.Tag.Resources.Banner.txt")).ReadToEnd();
            Console.WriteLine();
            Console.WriteLine(banner);
            Console.WriteLine();

            var repositoryPath = Repository.Discover(InitialRepositoryPath);
            if (repositoryPath == null)
            {
                _logger.LogInformation("No Git repository found.");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Found Repository at '{repositoryPath}'.");

            if (!Repository.IsValid(repositoryPath))
            {
                _logger.LogError("Current Directory is not a Git Repository.");
                return Task.CompletedTask;
            }

            using (var repository = new Repository(repositoryPath))
            {
                if (_git.IsDirty(repository))
                {
                    _logger.LogError("Working Directory is dirty. Commit your changes before tagging.");
                    return Task.CompletedTask;
                }

                var tagName = _nextTagService.GetNextTag(repository, SemVerElement);
                var annotation = string.Empty;

                if (!IsLightWeight)
                {
                    annotation = _annotationService.GetAnnotation(tagName);
                }

                _git.AddTag(tagName, annotation);
                _logger.LogInformation($"Added tag '{tagName}'");

                if (IsAutoPush || Prompt.GetYesNo("Push Tags?", true))
                {
                    _git.Push(repositoryPath);
                }
            }

            return Task.CompletedTask;
        }
    }
}