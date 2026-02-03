using System.Numerics;
using ACG.Model;
using AKG.FileHelper;
using AKG.Parser;
using NUnit.Framework;

namespace AKG.Test
{
    [TestFixture]
    public class ObjParserModelTests
    {
        private ObjModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new ObjModel();
        }

        [Test]
        public void AddVertices_ShouldStoreVerticesCorrectly()
        {
            // Arrange
            var vertices = new[]
            {
                new Vector4(1.0f, 2.0f, 3.0f, 1.0f),
                new Vector4(4.0f, 5.0f, 6.0f, 1.0f),
                new Vector4(7.0f, 8.0f, 9.0f, 1.0f)
            };

            // Act
            _model.AddVertices(vertices);
            var result = _model.Vertices;

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(vertices[0]));
            Assert.That(result[1], Is.EqualTo(vertices[1]));
            Assert.That(result[2], Is.EqualTo(vertices[2]));
        }

        [Test]
        public void AddNormals_ShouldStoreNormalsCorrectly()
        {
            // Arrange
            var normals = new[]
            {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };

            // Act
            _model.AddNormals(normals);
            var result = _model.Normals;

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(normals[0]));
            Assert.That(result[1], Is.EqualTo(normals[1]));
        }

        [Test]
        public void AddTextureCoords_ShouldStoreTextureCoordsCorrectly()
        {
            // Arrange
            var texCoords = new[]
            {
                new Vector2(0.0f, 0.0f),
                new Vector2(0.5f, 0.5f),
                new Vector2(1.0f, 1.0f)
            };

            // Act
            _model.AddTextureCoords(texCoords);
            var result = _model.TextureCoords;

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(texCoords[0]));
            Assert.That(result[1], Is.EqualTo(texCoords[1]));
            Assert.That(result[2], Is.EqualTo(texCoords[2]));
        }

        [Test]
        public void Clear_ShouldRemoveAllData()
        {
            // Arrange
            _model.AddVertices(new[] { new Vector4(1, 2, 3, 1) });
            _model.AddNormals(new[] { new Vector3(0, 1, 0) });
            _model.AddTextureCoords(new[] { new Vector2(0, 0) });

            // Act
            _model.Clear();

            // Assert
            Assert.That(_model.Vertices, Is.Empty);
            Assert.That(_model.Normals, Is.Empty);
            Assert.That(_model.TextureCoords, Is.Empty);
        }
    }

    [TestFixture]
    public class ObjParserTests
    {
        private ObjParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ObjParser();
        }

        [Test]
        public void Parse_ValidVertices_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 1.0 2.0 3.0
v 4.0 5.0 6.0 1.5
v -1.0 -2.0 -3.0
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.Vertices[0], Is.EqualTo(new Vector4(1.0f, 2.0f, 3.0f, 1.0f)));
            Assert.That(model.Vertices[1], Is.EqualTo(new Vector4(4.0f, 5.0f, 6.0f, 1.5f)));
            Assert.That(model.Vertices[2], Is.EqualTo(new Vector4(-1.0f, -2.0f, -3.0f, 1.0f)));
        }

        [Test]
        public void Parse_VertexNormals_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
vn 0.0 1.0 0.0
vn 1.0 0.0 0.0
vn 0.0 0.0 1.0
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Normals, Has.Count.EqualTo(3));
            Assert.That(model.Normals[0], Is.EqualTo(new Vector3(0.0f, 1.0f, 0.0f)));
            Assert.That(model.Normals[1], Is.EqualTo(new Vector3(1.0f, 0.0f, 0.0f)));
            Assert.That(model.Normals[2], Is.EqualTo(new Vector3(0.0f, 0.0f, 1.0f)));
        }

        [Test]
        public void Parse_TextureCoordinates_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
vt 0.0 0.0
vt 0.5 0.5
vt 1.0 0.0
vt 0.0 1.0
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.TextureCoords, Has.Count.EqualTo(4));
            Assert.That(model.TextureCoords[0], Is.EqualTo(new Vector2(0.0f, 0.0f)));
            Assert.That(model.TextureCoords[1], Is.EqualTo(new Vector2(0.5f, 0.5f)));
            Assert.That(model.TextureCoords[2], Is.EqualTo(new Vector2(1.0f, 0.0f)));
            Assert.That(model.TextureCoords[3], Is.EqualTo(new Vector2(0.0f, 1.0f)));
        }

        [Test]
        public void Parse_Faces_NoSlashes_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 0.0 0.0 0.0
v 1.0 0.0 0.0
v 0.0 1.0 0.0
f 1 2 3
f 1 3 2
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.Faces, Has.Count.EqualTo(2));
            
            // Check first face indices
            ACG.Model.FaceIndices[] face1 = model.Faces[0];
            Assert.That(face1, Has.Length.EqualTo(3));
            Assert.That(face1[0].VertexIndex, Is.EqualTo(1));
            Assert.That(face1[1].VertexIndex, Is.EqualTo(2));
            Assert.That(face1[2].VertexIndex, Is.EqualTo(3));
        }

        [Test]
        public void Parse_Faces_WithTextureOnly_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 0.0 0.0 0.0
v 1.0 0.0 0.0
v 0.0 1.0 0.0
vt 0.0 0.0
vt 1.0 0.0
vt 0.0 1.0
f 1/1 2/2 3/3
f 3/3 2/2 1/1
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.TextureCoords, Has.Count.EqualTo(3));
            Assert.That(model.Faces, Has.Count.EqualTo(2));
            
            // Check first face indices with texture coordinates
            var face1 = model.Faces[0];
            Assert.That(face1[0].VertexIndex, Is.EqualTo(1));
            Assert.That(face1[0].TextureIndex, Is.EqualTo(1));
            Assert.That(face1[1].VertexIndex, Is.EqualTo(2));
            Assert.That(face1[1].TextureIndex, Is.EqualTo(2));
            Assert.That(face1[2].VertexIndex, Is.EqualTo(3));
            Assert.That(face1[2].TextureIndex, Is.EqualTo(3));
        }

        [Test]
        public void Parse_Faces_WithNormalOnly_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 0.0 0.0 0.0
v 1.0 0.0 0.0
v 0.0 1.0 0.0
vn 0.0 0.0 1.0
vn 0.0 0.0 -1.0
f 1//1 2//2 3//1
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.Normals, Has.Count.EqualTo(2));
            Assert.That(model.Faces, Has.Count.EqualTo(1));
            
            // Check face indices with normals
            var face = model.Faces[0];
            Assert.That(face[0].VertexIndex, Is.EqualTo(1));
            Assert.That(face[0].NormalIndex, Is.EqualTo(1));
            Assert.That(face[1].VertexIndex, Is.EqualTo(2));
            Assert.That(face[1].NormalIndex, Is.EqualTo(2));
            Assert.That(face[2].VertexIndex, Is.EqualTo(3));
            Assert.That(face[2].NormalIndex, Is.EqualTo(1));
        }

        [Test]
        public void Parse_Faces_WithTextureAndNormal_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 0.0 0.0 0.0
v 1.0 0.0 0.0
v 0.0 1.0 0.0
vt 0.0 0.0
vt 1.0 0.0
vt 0.0 1.0
vn 0.0 0.0 1.0
vn 0.0 1.0 0.0
f 1/1/1 2/2/1 3/3/2
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(3));
            Assert.That(model.TextureCoords, Has.Count.EqualTo(3));
            Assert.That(model.Normals, Has.Count.EqualTo(2));
            Assert.That(model.Faces, Has.Count.EqualTo(1));
            
            // Check face indices with both texture and normal
            var face = model.Faces[0];
            Assert.That(face[0].VertexIndex, Is.EqualTo(1));
            Assert.That(face[0].TextureIndex, Is.EqualTo(1));
            Assert.That(face[0].NormalIndex, Is.EqualTo(1));
            Assert.That(face[1].VertexIndex, Is.EqualTo(2));
            Assert.That(face[1].TextureIndex, Is.EqualTo(2));
            Assert.That(face[1].NormalIndex, Is.EqualTo(1));
            Assert.That(face[2].VertexIndex, Is.EqualTo(3));
            Assert.That(face[2].TextureIndex, Is.EqualTo(3));
            Assert.That(face[2].NormalIndex, Is.EqualTo(2));
        }

        [Test]
        public void Parse_MixedContent_ShouldParseAllElementsCorrectly()
        {
            // Arrange
            string objContent = @"
# This is a comment
v 1.0 2.0 3.0
v 4.0 5.0 6.0

vn 0.0 1.0 0.0
vn 1.0 0.0 0.0

vt 0.0 0.0
vt 1.0 0.0

f 1/1/1 2/2/2
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(2));
            Assert.That(model.Normals, Has.Count.EqualTo(2));
            Assert.That(model.TextureCoords, Has.Count.EqualTo(2));
            Assert.That(model.Faces, Has.Count.EqualTo(1));
        }

        [Test]
        public void Parse_EmptyFile_ShouldReturnEmptyModel()
        {
            // Arrange
            string objContent = "# Empty OBJ file\n";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Is.Empty);
            Assert.That(model.Normals, Is.Empty);
            Assert.That(model.TextureCoords, Is.Empty);
            Assert.That(model.Faces, Is.Empty);
        }

        [Test]
        public void Parse_QuadFace_ShouldParseCorrectly()
        {
            // Arrange
            string objContent = @"
v 0.0 0.0 0.0
v 1.0 0.0 0.0
v 1.0 1.0 0.0
v 0.0 1.0 0.0
f 1 2 3 4
";
            var chunk = CreateFileChunk(objContent);

            // Act
            var model = _parser.Parse(chunk);

            // Assert
            Assert.That(model.Vertices, Has.Count.EqualTo(4));
            Assert.That(model.Faces, Has.Count.EqualTo(1));
            Assert.That(model.Faces[0], Has.Length.EqualTo(4)); // Quad face
        }

        private FileChunk CreateFileChunk(string content)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            return new FileChunk(buffer, buffer.Length);
        }
    }

    // You'll need to add this struct definition for tests if it's not accessible
    // This assumes FaceIndices is defined elsewhere in your codebase
    public struct FaceIndices
    {
        public int VertexIndex { get; set; }
        public int TextureIndex { get; set; }
        public int NormalIndex { get; set; }
    }
}