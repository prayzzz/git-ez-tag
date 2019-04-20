using System;
using System.Diagnostics;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace Git.Ez.Tag
{
    public class Git
    {
        private readonly ILogger<Git> _logger;

        public Git(ILogger<Git> logger)
        {
            _logger = logger;
        }

        public bool IsDirty(Repository repository)
        {
            return repository.Diff.Compare<TreeChanges>(repository.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory).Any();
        }

        public string GetLatestTag(Repository repository)
        {
            try
            {
                return repository.Describe(repository.Head.Tip);
            }
            catch (LibGit2SharpException)
            {
                return null;
            }
        }

        public void AddTag(string tagName, string annotation)
        {
            if (string.IsNullOrEmpty(annotation))
            {
                RunGit($"tag {tagName}");
            }
            else
            {
                RunGit($"tag -a {tagName} -m {annotation}");
            }
        }

        public void Push(string repositoryPath)
        {
            var result = RunGit("push --tags", repositoryPath);
            if (!result)
            {
                _logger.LogError("Command 'git push --tags' failed");
            }
        }

        private bool RunGit(string arguments, string workingDirectory = ".")
        {
            _logger.LogDebug($"Executing 'git {arguments}'");
            var processStartInfo = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = workingDirectory
            };

            var process = Process.Start(processStartInfo);
            if (process == null)
            {
                _logger.LogError("Couldn't start Git process.");
                return false;
            }

            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                _logger.LogDebug($"Execution of 'git {arguments}' successful");
                return true;
            }
            else
            {
                _logger.LogDebug($"Execution of 'git {arguments}' failed");
                return false;
            }
        }
    }
}