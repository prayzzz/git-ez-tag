using System;
using System.IO;
using System.Reflection;
using LibGit2Sharp;

namespace Git.Ez.Tag
{
    internal static class Program
    {
        private const string InitialRepositoryPath = ".";

        private static void Main()
        {
            var banner = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Git.Ez.Tag.Resources.Banner.txt")).ReadToEnd();
            Console.WriteLine(banner);
            Console.WriteLine();

            var repositoryPath = Repository.Discover(InitialRepositoryPath);
            if (repositoryPath == null)
            {
                Console.WriteLine("No Git repository found.");
                Environment.Exit(1);
            }

            Console.WriteLine($"Found Repository at '{repositoryPath}'.");

            if (!Repository.IsValid(repositoryPath))
            {
                Console.WriteLine("Current Directory is not a Git Repository.");
                Environment.Exit(1);
            }

            using (var repository = new Repository(repositoryPath))
            {
                if (Git.IsDirty(repository))
                {
                    Console.Write("Working Directory is dirty. Commit your changes before tagging.");
                    Environment.Exit(1);
                }

                var latestTag = Git.GetLatestTag(repository);
                var nextTagSuggestion = "";
                if (latestTag == null)
                {
                    nextTagSuggestion = "1.0.0";
                    Console.WriteLine("No Tag available");
                }
                else
                {
                    Console.WriteLine($"Latest Tag '{latestTag}'");

                    var semver = SemVersion.Parse(latestTag);
                    nextTagSuggestion = new SemVersion(semver.Major, semver.Minor + 1, semver.Patch).ToString();
                }

                Console.Write($"Next Tag [{nextTagSuggestion}]: ");
                var nextTag = Console.ReadLine();
                if (string.IsNullOrEmpty(nextTag))
                {
                    nextTag = nextTagSuggestion;
                }

                Console.Write($"Annotation for new Tag {nextTag}: ");
                var annotation = Console.ReadLine();
            }
        }
    }
}