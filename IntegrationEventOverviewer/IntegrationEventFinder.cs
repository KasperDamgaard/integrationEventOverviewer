using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace IntegrationEventOverviewer;

public class IntegrationEventFinder : IIntegrationEventFinder
{
    private const string IntegrationEventInterfaceName = "IIntegrationEvent";

    public async Task<IEnumerable<ClassDeclarationSyntax>> FindIntegrationEventImplementors(string pathToSolution)
    {
        // TODO: KLD: Move to some base class which set's up stuff
        MSBuildLocator.RegisterDefaults();
        
        if (!File.Exists(pathToSolution) || !pathToSolution.EndsWith(".sln"))
        {
            Console.Error.WriteLine("{0} is not a valid file", pathToSolution);
            throw new ArgumentException("Illegal argument - not a valid file", nameof(pathToSolution));
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);


        var implementors = new List<ClassDeclarationSyntax>(); 
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null)
            {
                throw new Exception("Compilation not found");
            }

            var classVisitor = new ClassVirtualizationVisitor(compilation);

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                classVisitor.Visit(await syntaxTree.GetRootAsync());
            }
            implementors.AddRange(classVisitor.Classes);
        }

        return implementors;
    }

    private class ClassVirtualizationVisitor : CSharpSyntaxRewriter
    {
        public List<ClassDeclarationSyntax> Classes { get; }
        private readonly Compilation _compilation;
        public ClassVirtualizationVisitor(Compilation compilation)
        {
            _compilation = compilation;
            Classes = new List<ClassDeclarationSyntax>();
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;
            if (ClassImplementsIntegrationEvent(node))
            {
                Classes.Add(node); // save your visited classes
            }
            return node;
        }
        
        private bool ClassImplementsIntegrationEvent(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var tree = classDeclarationSyntax.SyntaxTree;
            var root = tree.GetRoot();
            var sModel = _compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            var classSymbol = sModel.GetDeclaredSymbol(root.DescendantNodes().OfType<ClassDeclarationSyntax>().First());

            if (classSymbol == null) return false;
            var implementedInterfaces = classSymbol.AllInterfaces;
            foreach (var @interface in implementedInterfaces)
            {
                if (@interface.Name == IntegrationEventInterfaceName)
                {
                    return true;
                }
            }

            return false;
        }
    }

}