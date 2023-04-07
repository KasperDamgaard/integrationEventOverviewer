using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntegrationEventOverviewer;

public class ClassVirtualizationVisitor : CSharpSyntaxRewriter
{
    private readonly List<ClassDeclarationSyntax> _classes;
    private readonly Compilation _compilation;
    private readonly Func<INamedTypeSymbol,bool> _interfaceFilter;

    public ClassVirtualizationVisitor(Compilation compilation, Func<INamedTypeSymbol, bool> interfaceFilter)
    {
        _compilation = compilation;
        _classes = new List<ClassDeclarationSyntax>();
        _interfaceFilter = interfaceFilter;
    }

    public async Task<List<ClassDeclarationSyntax>> LocateInterfaces()
    {
        foreach (var syntaxTree in _compilation.SyntaxTrees)
        {
            Visit(await syntaxTree.GetRootAsync());
        }

        return _classes;
    } 

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;
        if (ClassImplementsIntegrationEvent(node))
        {
            _classes.Add(node); // save your visited classes
        }
        return node;
    }
    
    private bool ClassImplementsIntegrationEvent(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var sModel = _compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        var classSymbol = sModel.GetDeclaredSymbol(classDeclarationSyntax);
        // Only find classes that can be instantiated
        if (classSymbol is {IsAbstract: true})
        {
            return false;
        }
        return classSymbol != null && _interfaceFilter(classSymbol);
    }
}