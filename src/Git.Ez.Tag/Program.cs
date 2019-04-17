using System;
using System.IO;
using LibGit2Sharp;

namespace Git.Ez.Tag
{
    internal static class Program
    {
        private const string InitialRepositoryPath = ".";

        private static void Main()
        {
            Console.WriteLine($"Current Directory is '{Directory.GetCurrentDirectory()}'.");

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
                if (latestTag == null)
                {
                    Console.WriteLine("No Tag available");
                }
                else
                {
                    Console.WriteLine($"Latest Tag '{latestTag}'");
                }
            }
        }
    }
}