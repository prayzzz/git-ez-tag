using System.Linq;
using LibGit2Sharp;

namespace Git.Ez.Tag
{
    public static class Git
    {
        public static bool IsDirty(Repository repository)
        {
            return repository.Diff.Compare<TreeChanges>(repository.Head.Tip.Tree,DiffTargets.Index | DiffTargets.WorkingDirectory).Any();
        }
        
        public static string GetLatestTag(Repository repository)
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
    }
}