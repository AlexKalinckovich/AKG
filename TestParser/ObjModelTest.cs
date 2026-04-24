using System.Numerics;
using AKG.Core.Model;
using AKG.Model;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class ObjModelTests
{
    private ObjModel _model;

    [SetUp]
    public void Setup()
    {
        _model = new ObjModel();
    }

    [Test]
    public void AddVertices_SingleVertex_AddsCorrectly()
    {
        // Arrange
        var vertex = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);

        // Act
        _model.AddVertices(new[] { vertex });

        // Assert
        Assert.That(_model.Vertices, Has.Count.EqualTo(1));
        Assert.That(_model.Vertices[0], Is.EqualTo(vertex));
    }

    [Test]
    public void AddVertices_MultipleVertices_AddsAll()
    {
        // Arrange
        var vertices = new[]
        {
            new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
        };

        // Act
        _model.AddVertices(vertices);

        // Assert
        Assert.That(_model.Vertices, Has.Count.EqualTo(3));
    }

    [Test]
    public void AddNormals_AddsNormalsCorrectly()
    {
        // Arrange
        var normals = new[]
        {
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 0.0f)
        };

        // Act
        _model.AddNormals(normals);

        // Assert
        Assert.That(_model.Normals, Has.Count.EqualTo(2));
    }

    [Test]
    public void AddTextureCoords_AddsTexturesCorrectly()
    {
        // Arrange
        var textures = new[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.5f, 1.0f)
        };

        // Act
        _model.AddTextureCoords(textures);

        // Assert
        Assert.That(_model.TextureCoords, Has.Count.EqualTo(3));
    }

    [Test]
    public void AddFaces_TriangleFace_AddsCorrectly()
    {
        // Arrange
        var face = new[]
        {
            new FaceIndices(0, 0, 0),
            new FaceIndices(1, 1, 1),
            new FaceIndices(2, 2, 2)
        };

        // Act
        _model.AddFaces([new Face(face[0], face[1], face[2])]);

        // Assert
        Assert.That(_model.Faces, Has.Count.EqualTo(1));
        Assert.That(_model.Faces[0], Has.Length.EqualTo(3));
    }
    

    [Test]
    public void Properties_ReturnImmutableLists()
    {
        // Arrange
        _model.AddVertices(new[] { new Vector4(1.0f, 0.0f, 0.0f, 1.0f) });

        // Act & Assert
        Assert.That(_model.Vertices, Is.InstanceOf<IReadOnlyList<Vector4>>());
    }
}