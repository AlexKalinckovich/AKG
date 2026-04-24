using AKG.Core.Parser;
using AKG.Core.Parser.ObjPartParsers.LineParsers;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class FaceParserTests
{
    private FaceLineParser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new FaceLineParser();
    }

    [Test]
    public void ParseFaceLine_ValidTriangle_ReturnsThreeIndices()
    {
        // Arrange
        string faceLine = "1/2/3 4/5/6 7/8/9";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result.First.VertexIndex, Is.EqualTo(0));
            Assert.That(result.First.TextureIndex, Is.EqualTo(1));
            Assert.That(result.First.NormalIndex, Is.EqualTo(2));

            Assert.That(result.Second.VertexIndex, Is.EqualTo(3));
            Assert.That(result.Second.TextureIndex, Is.EqualTo(4));
            Assert.That(result.Second.NormalIndex, Is.EqualTo(5));

            Assert.That(result.Third.VertexIndex, Is.EqualTo(6));
            Assert.That(result.Third.TextureIndex, Is.EqualTo(7));
            Assert.That(result.Third.NormalIndex, Is.EqualTo(8));
        });
    }

    [Test]
    public void ParseFaceLine_VertexOnly_ReturnsCorrectIndices()
    {
        // Arrange
        string faceLine = "1 2 3";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result.First.VertexIndex, Is.EqualTo(0));
            Assert.That(result.First.TextureIndex, Is.EqualTo(0));
            Assert.That(result.First.NormalIndex, Is.EqualTo(0));

            Assert.That(result.Second.VertexIndex, Is.EqualTo(1));
            Assert.That(result.Second.TextureIndex, Is.EqualTo(0));
            Assert.That(result.Second.NormalIndex, Is.EqualTo(0));
        });
    }

    [Test]
    public void ParseFaceLine_VertexAndTexture_ReturnsCorrectIndices()
    {
        // Arrange
        string faceLine = "1/2 3/4 5/6";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result.First.VertexIndex, Is.EqualTo(0));
            Assert.That(result.First.TextureIndex, Is.EqualTo(1));
            Assert.That(result.First.NormalIndex, Is.EqualTo(0));

            Assert.That(result.Second.TextureIndex, Is.EqualTo(3));
        });
    }

    [Test]
    public void ParseFaceLine_VertexAndNormal_ReturnsCorrectIndices()
    {
        // Arrange
        string faceLine = "1//2 3//4 5//6";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result.First.VertexIndex, Is.EqualTo(0));
            Assert.That(result.First.TextureIndex, Is.EqualTo(0));
            Assert.That(result.First.NormalIndex, Is.EqualTo(1));

            Assert.That(result.Second.NormalIndex, Is.EqualTo(3));
        });
    }

    [Test]
    public void ParseFaceLine_QuadFace_ReturnsFourIndices()
    {
        // Arrange
        string faceLine = "1/2/3 4/5/6 7/8/9 10/11/12";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(4));
        Assert.That(result.Third.VertexIndex, Is.EqualTo(9));
    }

    [Test]
    public void ParseFaceLine_EmptyString_ReturnsEmptyArray()
    {
        // Arrange
        string faceLine = "";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseFaceLine_WhitespaceString_ReturnsEmptyArray()
    {
        // Arrange
        string faceLine = "   \t  ";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseFaceLine_InvalidFormat_HandlesGracefully()
    {
        // Arrange
        string faceLine = "1/abc/3 4//6";

        // Act
        var result = _parser.ParseFaceLineString(faceLine);

        // Assert
        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result.First.TextureIndex, Is.EqualTo(0)); // abc не парсится
        Assert.That(result.Second.NormalIndex, Is.EqualTo(5));  // 6-1 = 5
    }
}