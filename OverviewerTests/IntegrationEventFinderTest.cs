using System.Runtime.CompilerServices;
using IntegrationEventOverviewer;
using Shouldly;

namespace OverviewerTests;

public class IntegrationEventFinderTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        // Arrange
        var sut = new IntegrationEventFinder();
        var path = GetThisFilePath();
        var directory = Directory.GetParent(Path.GetDirectoryName(path)!)!.FullName;
        
        // Act
        var result = await sut.FindIntegrationEventImplementors(directory + Path.DirectorySeparatorChar + "IntegrationEventOverviewer.sln");
        
        // Assert
        result.ShouldContain(e => e.Identifier.Text == nameof(IntegrationEventTestImplementor));
    }

    private static string GetThisFilePath([CallerFilePath] string path = null!)
    {
        return path;
    }
}