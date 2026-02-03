using System.Numerics;
using AKG.Core.Model;
using AKG.Core.Parser;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class LineParserTests
{
    private LineParser _lineParser;

    [SetUp]
    public void Setup()
    {
        _lineParser = new LineParser();
    }

    [Test]
    public void ParseLine_VertexLine_AddsVertex()
    {
        // Arrange
        string line = "v 1.0 2.0 3.0";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Has.Count.EqualTo(1));
        Assert.That(result.Vertices[0], Is.EqualTo(new Vector4(1.0f, 2.0f, 3.0f, 1.0f)));
    }

    [Test]
    public void ParseLine_VertexWithWComponent_AddsVertex()
    {
        // Arrange
        string line = "v 1.0 2.0 3.0 0.5";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices[0].W, Is.EqualTo(0.5f));
    }

    [Test]
    public void ParseLine_TextureLine_AddsTextureCoord()
    {
        // Arrange
        string line = "vt 0.5 0.7";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.TextureCoords, Has.Count.EqualTo(1));
        Assert.That(result.TextureCoords[0], Is.EqualTo(new Vector2(0.5f, 0.7f)));
    }

    [Test]
    public void ParseLine_NormalLine_AddsNormal()
    {
        // Arrange
        string line = "vn 0.0 1.0 0.0";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Normals, Has.Count.EqualTo(1));
        Assert.That(result.Normals[0], Is.EqualTo(new Vector3(0.0f, 1.0f, 0.0f)));
    }

    [Test]
    public void ParseLine_FaceLine_AddsFace()
    {
        // Arrange
        string line = "f 1/2/3 4/5/6 7/8/9";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Faces, Has.Count.EqualTo(1));
        Assert.That(result.Faces[0], Has.Length.EqualTo(3));
        Assert.That(result.TotalFacesProcessed, Is.EqualTo(1));
    }

    [Test]
    public void ParseLine_MultipleVertexLines_AddsAllVertices()
    {
        // Arrange
        string[] lines = new[]
        {
            "v 1.0 0.0 0.0",
            "v 0.0 1.0 0.0",
            "v 0.0 0.0 1.0"
        };

        // Act
        foreach (var line in lines)
        {
            _lineParser.ParseLine(line);
        }
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Has.Count.EqualTo(3));
        Assert.That(result.TotalVerticesProcessed, Is.EqualTo(3));
    }

    [Test]
    public void ParseLine_CommentLine_Ignores()
    {
        // Arrange
        string line = "# This is a comment";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Is.Empty);
        Assert.That(result.Faces, Is.Empty);
    }

    [Test]
    public void ParseLine_EmptyLine_Ignores()
    {
        // Arrange
        string line = "";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Is.Empty);
    }

    [Test]
    public void ParseLine_LineWithTabs_ParsesCorrectly()
    {
        // Arrange
        string line = "v\t1.0\t2.0\t3.0";

        // Act
        _lineParser.ParseLine(line);
        ParticalModelData result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Has.Count.EqualTo(1));
    }

    [Test]
    public void ParseLine_InvalidVertexLine_DoesNotAddData()
    {
        // Arrange
        string line = "v invalid data";

        // Act
        _lineParser.ParseLine(line);
        var result = _lineParser.GetPartialModel();

        // Assert
        Assert.That(result.Vertices, Is.Empty);
    }
}