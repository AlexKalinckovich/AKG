
using AKG.Core.Model;
using AKG.Core.Parser;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class ChunkProcessorTests
{
    private ChunkProcessor _processor;

    [SetUp]
    public void Setup()
    {
        _processor = new ChunkProcessor();
    }

    [Test]
    public void ProcessChunk_SingleTriangle_ReturnsCorrectData()
    {
        // Arrange
        string[] lines = new[]
        {
            "v 1.0 0.0 0.0",
            "v 0.0 1.0 0.0",
            "v 0.0 0.0 1.0",
            "f 1 2 3"
        };

        // Act
        var result = _processor.ProcessChunk(lines);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Vertices, Has.Count.EqualTo(3));
            Assert.That(result.Faces, Has.Count.EqualTo(1));
            Assert.That(result.Faces[0], Has.Length.EqualTo(3));
            Assert.That(result.TotalVerticesProcessed, Is.EqualTo(3));
            Assert.That(result.TotalFacesProcessed, Is.EqualTo(1));
        });
    }

    [Test]
    public void ProcessChunk_MultipleFaces_ReturnsCorrectData()
    {
        // Arrange
        string[] lines = new[]
        {
            "v 0.0 0.0 0.0",
            "v 1.0 0.0 0.0",
            "v 0.0 1.0 0.0",
            "v 1.0 1.0 0.0",
            "f 1 2 3",
            "f 2 3 4"
        };

        // Act
        var result = _processor.ProcessChunk(lines);

        // Assert
        Assert.That(result.Faces, Has.Count.EqualTo(2));
        Assert.That(result.TotalFacesProcessed, Is.EqualTo(2));
    }

    [Test]
    public void ProcessChunk_WithTexturesAndNormals_ReturnsCompleteData()
    {
        // Arrange
        string[] lines = new[]
        {
            "v 0.0 0.0 0.0",
            "vt 0.0 0.0",
            "vn 0.0 0.0 1.0",
            "v 1.0 0.0 0.0",
            "vt 1.0 0.0",
            "vn 0.0 0.0 1.0",
            "v 0.0 1.0 0.0",
            "vt 0.0 1.0",
            "vn 0.0 0.0 1.0",
            "f 1/1/1 2/2/2 3/3/3"
        };

        // Act
        var result = _processor.ProcessChunk(lines);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Vertices, Has.Count.EqualTo(3));
            Assert.That(result.TextureCoords, Has.Count.EqualTo(3));
            Assert.That(result.Normals, Has.Count.EqualTo(3));
            Assert.That(result.Faces, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void ProcessChunk_EmptyLinesAndComments_IgnoresThem()
    {
        // Arrange
        string[] lines = new[]
        {
            "",
            "# Comment line",
            " ",
            "v 1.0 0.0 0.0",
            "\t",
            "# Another comment",
            "v 0.0 1.0 0.0",
            "f 1 2"
        };

        // Act
        var result = _processor.ProcessChunk(lines);

        // Assert
        Assert.That(result.Vertices, Has.Count.EqualTo(2));
        Assert.That(result.Faces, Has.Count.EqualTo(0)); // f 1 2 - это не грань (нужно минимум 3)
    }

    [Test]
    public void ProcessChunk_MultipleChunks_ResetsState()
    {
        // Arrange
        string[] chunk1 = new[] { "v 1.0 0.0 0.0" };
        string[] chunk2 = new[] { "v 0.0 1.0 0.0" };

        // Act
        ParticalModelData result1 = _processor.ProcessChunk(chunk1);
        ParticalModelData result2 = _processor.ProcessChunk(chunk2);

        // Assert
        Assert.That(result1.Vertices, Has.Count.EqualTo(1));
        Assert.That(result2.Vertices, Has.Count.EqualTo(1));
        Assert.That(result2.Vertices[0].Y, Is.EqualTo(1.0f));
    }

    [Test]
    public void ProcessChunk_InvalidData_ContinuesProcessing()
    {
        // Arrange
        string[] lines = new[]
        {
            "v 1.0 0.0 0.0",
            "invalid line",
            "v 0.0 1.0 0.0",
            "f invalid face",
            "f 1 2"
        };

        // Act
        var result = _processor.ProcessChunk(lines);

        // Assert
        Assert.That(result.Vertices, Has.Count.EqualTo(2));
        // f 1 2 - это не грань, т.к. нужно минимум 3 вершины
        Assert.That(result.Faces, Is.Empty);
    }
}