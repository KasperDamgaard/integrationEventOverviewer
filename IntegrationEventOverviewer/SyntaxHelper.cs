using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace IntegrationEventOverviewer;

public static class SyntaxHelper
{
    internal static string GetNamespaceFrom(SyntaxNode s) =>
        s.SyntaxTree.GetRoot().DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().First().Name.ToString();
    
    public static async Task<IEnumerable<Project>> GetProjects(string pathToSolution)
    {
        if (!File.Exists(pathToSolution) || !pathToSolution.EndsWith(".sln"))
        {
            throw new ArgumentException($"Illegal argument - {pathToSolution} is not a valid file", nameof(pathToSolution));
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);
        return solution.Projects.Where(p => p.Name.EndsWith("Tests", StringComparison.InvariantCultureIgnoreCase)).ToList();
    }
}