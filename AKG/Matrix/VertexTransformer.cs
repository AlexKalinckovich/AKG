using System.Numerics;
using AKG.Core.Model;
using AKG.Model.Vertex;

namespace AKG.Matrix;

public sealed class VertexTransformer
{
    private readonly VertexTransformCalculator _transformCalculator;
    private readonly VertexDataBufferManager _bufferManager;

    public VertexTransformer(
        TransformationMatrixManager matrixManager,
        int viewportWidth,
        int viewportHeight)
    {
        _transformCalculator = new VertexTransformCalculator(matrixManager, viewportWidth, viewportHeight);
        _bufferManager = new VertexDataBufferManager();
    }

    public ReadOnlyMemory<VertexData> TransformVertices(in ObjModel model)
    {
        int vertexCount = model.Vertices.Count;
        
        VertexData[] vertexBuffer = _bufferManager.GetOrCreateBuffer(vertexCount);

        ProcessVertices(model, vertexBuffer);

        return new ReadOnlyMemory<VertexData>(vertexBuffer, start: 0, length: vertexCount);
    }
    private void ProcessVertices(
        ObjModel model,
        VertexData[] buffer)
    {
        
        int vertexCount = model.Vertices.Count;
        
        Parallel.For(0, vertexCount, index =>
        {
            int faceArrayIndex = index / 3;
            int faceCellIndex = index % 3;
            
            int textureIndex = model.Faces[faceArrayIndex][faceCellIndex].TextureIndex;
            int normalIndex = model.Faces[faceArrayIndex][faceCellIndex].NormalIndex;
            
            Vector3 normal = GetNormal(model.Normals,normalIndex);
            
            Vector4 vertex = GetVertex(model.Vertices, index);
            
            Vector2 textureCoords = GetTextureCoordinate(model.TextureCoords, textureIndex);
            
            buffer[index] = _transformCalculator.Transform(vertex, normal, textureCoords);  
        });
    }
    
    private static Vector3 GetNormal(IReadOnlyList<Vector3> normals, int index)
    {
        return index < normals.Count ? normals[index] : Vector3.UnitZ;
    }
    
    private static Vector4 GetVertex(IReadOnlyList<Vector4> vertices, int index)
    {
        return index < vertices.Count ? vertices[index] : Vector4.UnitZ;
    }

    private static Vector2 GetTextureCoordinate(IReadOnlyList<Vector2> textureCoordinates, int index)
    {
        return index < textureCoordinates.Count ? textureCoordinates[index] : Vector2.Zero;
    }
}