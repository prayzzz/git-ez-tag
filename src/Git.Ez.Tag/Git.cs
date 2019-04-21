using System.Diagnostics;
using System.IO;
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

        public bool IsDirty(DirectoryInfo repository)
        {
            var (isClean, _, _) = RunGit("diff --no-ext-diff --quiet --exit-code", repository);
            return !isClean;
        }

        public string GetLatestTag(DirectoryInfo repository)
        {
            var (isSuccess, stdOut, stdError) = RunGit("describe --abbrev=0", repository);
            if (isSuccess)
            {
                return stdOut;
            }
            else
            {
                _logger.LogError($"Couldn't get latest Tag: '{stdError.GetFirstLine()}'");
                return "1.0.0";
            }
        }

        public void AddTag(DirectoryInfo repository, string tagName, string annotation)
        {
            var (isSuccess, _, stdError) = AddTagInternal(repository, tagName, annotation);
            if (!isSuccess)
            {
                _logger.LogError($"Couldn't add Tag: '{stdError.GetFirstLine()}'");
            }
        }

        private (bool IsSuccess, string StdOut, string StdError) AddTagInternal(DirectoryInfo repository, string tagName, string annotation)
        {
            if (string.IsNullOrEmpty(annotation))
            {
                return RunGit($"tag {tagName}", repository);
            }
            else
            {
                return RunGit($"tag -a {tagName} -m {annotation}", repository);
            }
        }

        public void Push(DirectoryInfo repository)
        {
            var (isSuccess, _, stdError) = RunGit("push --tags", repository);
            if (!isSuccess)
            {
                _logger.LogError($"Couldn't push Tags: '{stdError.GetFirstLine()}'");
            }
        }
        
        private (bool IsSuccess, string StdOut, string StdError) RunGit(string arguments, DirectoryInfo workingDirectory)
        {
            _logger.LogDebug($"Executing 'git {arguments}'");
            var processStartInfo = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = workingDirectory.ToString(),
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(processStartInfo);
            if (process == null)
            {
                _logger.LogError("Couldn't start Git process.");
                return (false, string.Empty, string.Empty);
            }

            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                _logger.LogDebug($"Execution of 'git {arguments}' successful");
                return (true, process.StandardOutput.ReadToEnd().Trim(), string.Empty);
            }
            else
            {
                _logger.LogDebug($"Execution of 'git {arguments}' failed");
                return (false, string.Empty, process.StandardError.ReadToEnd());
            }
        }
    }
}