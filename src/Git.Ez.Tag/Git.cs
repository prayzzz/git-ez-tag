using System;
using System.Linq;
using LibGit2Sharp;

namespace Git.Ez.Tag
{
    public class Git
    {
        public bool IsDirty(Repository repository)
        {
            return repository.Diff.Compare<TreeChanges>(repository.Head.Tip.Tree, DiffTargets.Index).Any();
        }
        
        public string GetLatestTag(Repository repository)
        {
            try
            {
                return repository.Describe(repository.Head.Tip);
            }
            catch (LibGit2SharpException e)
            {
                return null;
            }
        }
    }
}