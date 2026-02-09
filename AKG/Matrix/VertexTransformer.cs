using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Matrix;

public sealed class VertexTransformer : IDisposable
{
    private readonly TransformationMatrixManager _matrixManager;
    private readonly VertexDataBufferManager _bufferManager;
    
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;

    public VertexTransformer(TransformationMatrixManager matrixManager, int viewportWidth, int viewportHeight)
    {
        _matrixManager = matrixManager;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        _bufferManager = new VertexDataBufferManager();
    }

    public void TransformVertices(IReadOnlyList<Vector4> vertices)
    {
        int vertexCount = vertices.Count;
        VertexData[] vertexBuffer = _bufferManager.GetOrCreateBuffer(vertexCount);
        
        float halfWidth = _viewportWidth * RenderConstants.InitialHalfCoordinateSystem;
        float halfHeight = _viewportHeight * RenderConstants.InitialHalfCoordinateSystem;

        InitializeBufferWithDefaultValues(vertexBuffer, vertexCount);
        TransformVerticesInParallel(vertices, vertexBuffer, halfWidth, halfHeight);
    }

    private void InitializeBufferWithDefaultValues(VertexData[] buffer, int vertexCount)
    {
        for (int index = 0; index < vertexCount; index++)
        {
            buffer[index] = new VertexData
            {
                ScreenPoint = new Point(-1, -1),
                WorldPosition = Vector3.Zero,
                ViewPosition = Vector3.Zero,
                ClipPosition = Vector3.Zero,
                Depth = 0
            };
        }
    }

    private void TransformVerticesInParallel(IReadOnlyList<Vector4> vertices, VertexData[] buffer, float halfWidth, float halfHeight)
    {
        int vertexCount = vertices.Count;

        Parallel.For(0, vertexCount, index =>
        {
            TransformSingleVertex(vertices[index], index, buffer, halfWidth, halfHeight);
        });
    }

    private void TransformSingleVertex(Vector4 vertex, int index, VertexData[] buffer, float halfWidth, float halfHeight)
    {
        Vector4 worldPosition = _matrixManager.ModelMatrix.TransformPoint(vertex);
        Vector4 viewPosition = _matrixManager.ViewMatrix.TransformPoint(worldPosition);
        Vector4 clipPosition = _matrixManager.ProjectionMatrix.TransformPoint(viewPosition);

        Point screenPoint = ConvertClipSpaceToScreenSpace(clipPosition, halfWidth, halfHeight);
        
        buffer[index] = new VertexData
        {
            WorldPosition = new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z),
            ViewPosition = new Vector3(viewPosition.X, viewPosition.Y, viewPosition.Z),
            ClipPosition = new Vector3(clipPosition.X, clipPosition.Y, clipPosition.Z),
            ScreenPoint = screenPoint,
            Depth = clipPosition.Z / clipPosition.W
        };
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
}