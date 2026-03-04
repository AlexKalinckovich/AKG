using System.Numerics;
using System.Windows;
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

    public void TransformVertices(
        IReadOnlyList<Vector4> vertices,
        IReadOnlyList<Vector3> normals)
    {
        int vertexCount = vertices.Count;
        VertexData[] vertexBuffer = _bufferManager.GetOrCreateBuffer(vertexCount);

        ProcessVertices(vertices, normals, vertexBuffer);
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
        IReadOnlyList<Vector4> vertices,
        IReadOnlyList<Vector3> normals,
        VertexData[] buffer)
    {
        int vertexCount = vertices.Count;
        Parallel.For(0, vertexCount, index =>
        {
            Vector3 normal = GetNormal(normals,index);
            Vector4 vertex = GetVertex(vertices, index);
            buffer[index] = _transformCalculator.Transform(vertex, normal);
        });
    }
    
    private Vector3 GetNormal(IReadOnlyList<Vector3> normals, int index)
    {
        return normals.Count > 0 ? normals[index] : Vector3.UnitZ;
    }
    
    private Vector4 GetVertex(IReadOnlyList<Vector4> vertices, int index)
    {
        return vertices.Count > 0 ? vertices[index] : Vector4.UnitZ;
    }
}