// ================ VertexTransformer.cs ================

using System.Windows;
using AKG.Render;
using AKG.Render.Buffers;
using AKG.Render.Constants;

namespace AKG;

using System.Numerics;
using AKG.Model;

public sealed class VertexTransformer : IDisposable
{
    private readonly TransformationMatrixManager _matrixManager;
    private readonly ScreenPointBufferManager _bufferManager;
    
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;

    public VertexTransformer(TransformationMatrixManager matrixManager, int viewportWidth, int viewportHeight)
    {
        _matrixManager = matrixManager;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        _bufferManager = new ScreenPointBufferManager();
    }

    public void TransformVertices(IReadOnlyList<Vector4> vertices)
    {
        int vertexCount = vertices.Count;
        Point[] pointBuffer = _bufferManager.GetOrCreateBuffer(vertexCount);
        
        float halfWidth = _viewportWidth * RenderConstants.InitialHalfCoordinateSystem;
        float halfHeight = _viewportHeight * RenderConstants.InitialHalfCoordinateSystem;

        InitializeBufferWithDefaultValues(pointBuffer, vertexCount);
        TransformVerticesInParallel(vertices, pointBuffer, halfWidth, halfHeight);
    }

    private void InitializeBufferWithDefaultValues(Point[] buffer, int vertexCount)
    {
        for (int index = 0; index < vertexCount; index++)
        {
            buffer[index] = new Point(-1, -1);
        }
    }

    private void TransformVerticesInParallel(IReadOnlyList<Vector4> vertices, Point[] buffer, float halfWidth, float halfHeight)
    {
        int vertexCount = vertices.Count;

        Parallel.For(0, vertexCount, index =>
        {
            TransformSingleVertex(vertices[index], index, buffer, halfWidth, halfHeight);
        });
    }

    private void TransformSingleVertex(Vector4 vertex, int index, Point[] buffer, float halfWidth, float halfHeight)
    {
        Vector4 worldPosition = _matrixManager.ModelMatrix.TransformPoint(vertex);
        Vector4 viewPosition = _matrixManager.ViewMatrix.TransformPoint(worldPosition);
        Vector4 clipPosition = _matrixManager.ProjectionMatrix.TransformPoint(viewPosition);

        Point screenPoint = ConvertClipSpaceToScreenSpace(clipPosition, halfWidth, halfHeight);
        buffer[index] = screenPoint;
    }

    private Point ConvertClipSpaceToScreenSpace(Vector4 clipPosition, float halfWidth, float halfHeight)
    {
        if (IsDepthInvalid(clipPosition))
        {
            return new Point(-1, -1);
        }

        float normalizedDeviceCoordinateX = clipPosition.X / clipPosition.W;
        float normalizedDeviceCoordinateY = clipPosition.Y / clipPosition.W;

        if (IsCoordinateOutsideViewableRange(normalizedDeviceCoordinateX, normalizedDeviceCoordinateY))
        {
            return new Point(-1, -1);
        }

        int screenX = ConvertNormalizedToScreenCoordinateX(normalizedDeviceCoordinateX, halfWidth);
        int screenY = ConvertNormalizedToScreenCoordinateY(normalizedDeviceCoordinateY, halfHeight);

        screenX = Math.Clamp(screenX, 0, _viewportWidth - 1);
        screenY = Math.Clamp(screenY, 0, _viewportHeight - 1);

        return new Point(screenX, screenY);
    }

    private bool IsDepthInvalid(Vector4 clipPosition)
    {
        return Math.Abs(clipPosition.W) < RenderConstants.DepthDivisionThreshold;
    }

    private bool IsCoordinateOutsideViewableRange(float x, float y)
    {
        float threshold = RenderConstants.CoordinateSystemNormalizationFactor;
        return x < -threshold || x > threshold || y < -threshold || y > threshold;
    }

    private int ConvertNormalizedToScreenCoordinateX(float normalizedCoordinate, float halfDimension)
    {
        return (int)((normalizedCoordinate + 1) * halfDimension);
    }

    private int ConvertNormalizedToScreenCoordinateY(float normalizedCoordinate, float halfDimension)
    {
        return (int)((1 - normalizedCoordinate) * halfDimension);
    }

    public Point[] GetTransformedPoints()
    {
        return _bufferManager.GetBufferArray();
    }

    public int GetTransformedPointsCount()
    {
        return _bufferManager.CurrentBufferSize;
    }

    public void Dispose()
    {
        _bufferManager.Dispose();
    }
}