using AKG.Core.Parser;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class ObjParserTests
{
    private ObjParser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new ObjParser();
    }

    [Test]
    public async Task ParseAsync_SingleChunk_ReturnsCompleteModel()
    {
        // Arrange
        async IAsyncEnumerable<string[]> GetChunks()
        {
            yield return new[]
            {
                "v 1.0 0.0 0.0",
                "v 0.0 1.0 0.0",
                "v 0.0 0.0 1.0",
                "f 1 2 3"
            };
            await Task.CompletedTask;
        }

        // Act
        var model = await _parser.ParseAsync(GetChunks());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.Faces, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task ParseAsync_MultipleChunks_MergesData()
    {
        // Arrange
        async IAsyncEnumerable<string[]> GetChunks()
        {
            yield return new[]
            {
                "v 1.0 0.0 0.0",
                "v 0.0 1.0 0.0"
            };
            yield return new[]
            {
                "v 0.0 0.0 1.0",
                "f 1 2 3"
            };
            await Task.CompletedTask;
        }

        // Act
        var model = await _parser.ParseAsync(GetChunks());

        // Assert
        Assert.That(model.Vertices, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task ParseAsync_EmptyChunks_ReturnsEmptyModel()
    {
        // Arrange
        async IAsyncEnumerable<string[]> GetChunks()
        {
            yield return new string[0];
            await Task.CompletedTask;
        }

        // Act
        var model = await _parser.ParseAsync(GetChunks());

        // Assert
        Assert.That(model.Vertices, Is.Empty);
    }

    [Test]
    public async Task ParseAsync_LargeChunks_ProcessesAllData()
    {
        // Arrange
        async IAsyncEnumerable<string[]> GetChunks()
        {
            var chunk = new string[300];
            for (int i = 0; i < 300; i++)
            {
                chunk[i] = $"v {i}.0 {i}.0 {i}.0";
            }
            yield return chunk;
            await Task.CompletedTask;
        }

        // Act
        var model = await _parser.ParseAsync(GetChunks());

        // Assert
        Assert.That(model.Vertices, Has.Count.EqualTo(300));
    }
}