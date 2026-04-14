using System.Numerics;
using System.Windows;
using AKG.Core.Model;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Matrix;

public sealed class VertexTransformer : IDisposable
{
    private readonly VertexTransformCalculator _transformCalculator;
    private readonly VertexDataBufferManager _bufferManager;

    public VertexTransformer(
        TransformationMatrixManager matrixManager,
        int viewportWidth,
        int viewportHeight)
    {
        ScreenSpaceConverter screenConverter = new ScreenSpaceConverter(viewportWidth, viewportHeight);
        _transformCalculator = new VertexTransformCalculator(matrixManager, screenConverter);
        _bufferManager = new VertexDataBufferManager();
    }

    public void TransformVertices(in ObjModel model)
    {
        int vertexCount = model.Vertices.Count;
        VertexData[] vertexBuffer = _bufferManager.GetOrCreateBuffer(vertexCount);

        ProcessVertices(model, vertexBuffer);
    }

    public VertexData[] GetTransformedVertices()
    {
        return _bufferManager.GetBufferArray();
    }

    public int GetTransformedVerticesCount()
    {
        return _bufferManager.CurrentBufferSize;
    }

    public void Dispose()
    {
        _bufferManager.Dispose();
    }
    
    private void ProcessVertices(
        ObjModel model,
        VertexData[] buffer)
    {
        
        int vertexCount = model.Vertices.Count;
        
        Parallel.For(0, vertexCount, index =>
        {
            Vector3 normal = GetNormal(model.Normals,index);
            Vector4 vertex = GetVertex(model.Vertices, index);
            Vector2 textureCoords = GetTextureCoordinate(model.TextureCoords, index);
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