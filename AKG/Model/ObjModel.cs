using System.Collections.Concurrent;
using System.Numerics;

namespace ACG.Model;

public class ObjModel
{
    private readonly ConcurrentBag<Vector4> _vertices = new();
    private readonly ConcurrentBag<Vector3> _normals = new();
    private readonly ConcurrentBag<Vector2> _textureCoords = new();
    private readonly ConcurrentBag<FaceIndices[]> _faces = new();
    
    private readonly object _verticesLock = new();
    private readonly object _normalsLock = new();
    private readonly object _textureCoordsLock = new();
    private readonly object _facesLock = new();

    public IReadOnlyList<Vector4> Vertices
    {
        get
        {
            lock (_verticesLock)
            {
                return _vertices.ToList();
            }
        }
    }

    public IReadOnlyList<Vector3> Normals
    {
        get
        {
            lock (_normalsLock)
            {
                return _normals.ToList();
            }
        }
    }

    public IReadOnlyList<Vector2> TextureCoords
    {
        get
        {
            lock (_textureCoordsLock)
            {
                return _textureCoords.ToList();
            }
        }
    }

    public IReadOnlyList<FaceIndices[]> Faces
    {
        get
        {
            lock (_facesLock)
            {
                return _faces.ToList();
            }
        }
    }

    public void AddVertices(IEnumerable<Vector4> vertices)
    {
        foreach (Vector4 vertex in vertices)
        {
            _vertices.Add(vertex);
        }
    }

    public void AddNormals(IEnumerable<Vector3> normals)
    {
        foreach (Vector3 normal in normals)
        {
            _normals.Add(normal);
        }
    }

    public void AddTextureCoords(IEnumerable<Vector2> textureCoords)
    {
        foreach (Vector2 textureCoord in textureCoords)
        {
            _textureCoords.Add(textureCoord);
        }
    }

    public void AddFaces(IEnumerable<FaceIndices[]> faces)
    {
        foreach (FaceIndices[] face in faces)
        {
            _faces.Add(face);
        }
    }

    public void Clear()
    {
        lock (_verticesLock) _vertices.Clear();
        lock (_normalsLock) _normals.Clear();
        lock (_textureCoordsLock) _textureCoords.Clear();
        lock (_facesLock) _faces.Clear();
    }
}