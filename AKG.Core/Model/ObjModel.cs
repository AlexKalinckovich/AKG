using System.Numerics;

namespace AKG.Core.Model;

public class ObjModel
{
    private readonly List<Vector4> _vertices = new(1000);
    private readonly List<Vector3> _normals = new(1000);
    private readonly List<Vector2> _textureCoords = new(1000);
    private readonly List<FaceIndices[]> _faces = new(1000);

    public IReadOnlyList<Vector4> Vertices => _vertices;
    public IReadOnlyList<Vector3> Normals => _normals;
    public IReadOnlyList<Vector2> TextureCoords => _textureCoords;
    public IReadOnlyList<FaceIndices[]> Faces => _faces;

    public void AddVertices(IEnumerable<Vector4> vertices)
    {
        _vertices.AddRange(vertices);
    }

    public void AddNormals(IEnumerable<Vector3> normals)
    {
        _normals.AddRange(normals);
    }

    public void AddTextureCoords(IEnumerable<Vector2> textureCoords)
    {
        _textureCoords.AddRange(textureCoords);
    }

    public void AddFaces(IEnumerable<FaceIndices[]> faces)
    {
        _faces.AddRange(faces);
    }

    public void MergeWith(PartialModelData partialData)
    {
        AddVertices(partialData.Vertices);
        AddNormals(partialData.Normals);
        AddTextureCoords(partialData.TextureCoords);
        AddFaces(partialData.Faces);
    }

    public void Clear()
    {
        _vertices.Clear();
        _normals.Clear();
        _textureCoords.Clear();
        _faces.Clear();
    }
}