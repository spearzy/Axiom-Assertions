namespace Axiom.Docs.Snippets.Tests;

internal static class RepositoryPaths
{
    private static readonly Lazy<string> RootPath = new(ResolveRootPath);

    public static string Root => RootPath.Value;

    public static string DocsDirectory => Path.Combine(Root, "docs");

    private static string ResolveRootPath()
    {
        // Test output paths move around, so walk upward until we hit the repo root.
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var solutionPath = Path.Combine(directory.FullName, "Axiom.sln");
            var docsPath = Path.Combine(directory.FullName, "docs");
            if (File.Exists(solutionPath) && Directory.Exists(docsPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the Axiom repository root from the test assembly directory.");
    }
}
