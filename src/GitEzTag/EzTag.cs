using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GitEzTag.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace GitEzTag
{
    [Command("ez-tag")]
    internal class EzTag
    {
        private static readonly DirectoryInfo InitialRepositoryPath = new DirectoryInfo(Directory.GetCurrentDirectory());
        private readonly AnnotationService _annotationService;
        private readonly IConsole _console;
        private readonly Git _git;

        private readonly ILogger<EzTag> _logger;
        private readonly TagService _tagService;

        public EzTag(ILogger<EzTag> logger, IConsole console, Git git, TagService tagService, AnnotationService annotationService)
        {
            _logger = logger;
            _console = console;
            _git = git;
            _tagService = tagService;
            _annotationService = annotationService;
        }

        [Option("-p|--push", "Executes 'git push --tags' after adding the Tag", CommandOptionType.NoValue)]
        public bool IsAutoPush { get; set; }

        [Option("-i|--increase", "Auto increase the given part of the version", CommandOptionType.SingleValue, ValueName = "Major|Minor|Patch")]
        public SemanticVersionElement SemanticVersionElement { get; set; }

        [Option("-l|--lightweight", "Skip Tag annotation", CommandOptionType.NoValue)]
        public bool IsLightWeight { get; set; }

        private static DirectoryInfo DiscoverGitDir(DirectoryInfo currentDirectory)
        {
            while (true)
            {
                if (currentDirectory.EnumerateDirectories().Any(d => d.Name == ".git")) return currentDirectory;

                if (currentDirectory.Parent == null) return null;

                currentDirectory = currentDirectory.Parent;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private Task OnExecuteAsync(CancellationToken ct)
        {
            try
            {
                return Execute(ct);
            }
            catch (OperationCanceledException)
            {
            }

            return Task.CompletedTask;
        }

        private Task Execute(CancellationToken ct)
        {
            var banner = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("GitEzTag.Resources.Banner.txt")).ReadToEnd();
            _console.WriteLine();
            _console.WriteLine(banner);
            _console.WriteLine();

            var repositoryDirectory = DiscoverGitDir(InitialRepositoryPath);
            if (repositoryDirectory == null)
            {
                _logger.LogError("No Git repository found.");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Found Repository at '{repositoryDirectory.FullName}'.");

            if (_git.IsDirty(repositoryDirectory))
            {
                _logger.LogError("Working Directory is dirty. Commit your changes before tagging.");
                return Task.CompletedTask;
            }

            var tagName = _tagService.GetAndIncrementTag(repositoryDirectory, SemanticVersionElement);
            ThrowIfCancellationRequested(ct);

            _logger.LogInformation($"New Tag will be '{tagName}'");

            var annotation = string.Empty;
            if (!IsLightWeight) annotation = _annotationService.GetAnnotation(tagName);

            ThrowIfCancellationRequested(ct);

            _git.AddTag(repositoryDirectory, tagName, annotation);
            _logger.LogInformation($"Added tag '{tagName}'");

            if (IsAutoPush || Prompt.GetYesNo($"> Push Tag '{tagName}' now?", true))
            {
                ThrowIfCancellationRequested(ct);

                _git.PushTag(repositoryDirectory, tagName);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Cancellation request is set asynchronous. Wait a short amount of time before checking the Token.
        /// </summary>
        private static void ThrowIfCancellationRequested(CancellationToken ct)
        {
            Thread.Sleep(10);
            ct.ThrowIfCancellationRequested();
        }
    }
}