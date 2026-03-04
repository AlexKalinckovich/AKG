using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Drawing;
using AKG.Render.Lighting;

namespace AKG.Render.Rasterization;


public sealed class TriangleRasterizer
{
    private readonly int _width;
    private readonly int _height;
    private readonly ZBufferManager _zBufferManager;
    private readonly BarycentricCalculator _barycentricCalculator;
    private readonly LightingCalculator _lightingCalculator;
    private readonly PixelDrawer _pixelDrawer;

    public TriangleRasterizer(
        int width,
        int height,
        ZBufferManager zBufferManager,
        BarycentricCalculator barycentricCalculator,
        LightingCalculator lightingCalculator,
        PixelDrawer pixelDrawer)
    {
        _width = width;
        _height = height;
        _zBufferManager = zBufferManager;
        _barycentricCalculator = barycentricCalculator;
        _lightingCalculator = lightingCalculator;
        _pixelDrawer = pixelDrawer;
    }

    public void Rasterize(VertexData vertex0, VertexData vertex1, VertexData vertex2, Vector3 cameraPosition)
    {
        Point point0 = vertex0.ScreenPoint;
        Point point1 = vertex1.ScreenPoint;
        Point point2 = vertex2.ScreenPoint;

        int minX = CalculateMinX(point0, point1, point2);
        int maxX = CalculateMaxX(point0, point1, point2);
        int minY = CalculateMinY(point0, point1, point2);
        int maxY = CalculateMaxY(point0, point1, point2);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                ProcessPixel(x, y, point0, point1, point2, vertex0, vertex1, vertex2, cameraPosition);
            }
        }
    }

    private void ProcessPixel(
        int x, int y,
        Point point0, Point point1, Point point2,
        VertexData vertex0, VertexData vertex1, VertexData vertex2,
        Vector3 cameraPosition)
    {
        if (!_barycentricCalculator.IsPointInTriangle(x, y, point0, point1, point2))
        {
            return;
        }

        _barycentricCalculator.ComputeBarycentricCoordinates(
            x, y, point0, point1, point2,
            out float weight0, out float weight1, out float weight2);

        float depth = weight0 * vertex0.Depth + weight1 * vertex1.Depth + weight2 * vertex2.Depth;

        if (!_zBufferManager.ShouldUpdatePixel(x, y, depth))
        {
            return;
        }

        _zBufferManager.UpdateDepth(x, y, depth);

        Vector3 interpolatedNormal = weight0 * vertex0.Normal + weight1 * vertex1.Normal + weight2 * vertex2.Normal;
        Vector3 interpolatedWorldPos = weight0 * vertex0.WorldPosition + 
                                        weight1 * vertex1.WorldPosition + 
                                        weight2 * vertex2.WorldPosition;

        uint pixelColor = _lightingCalculator.CalculatePixelColor(interpolatedWorldPos, interpolatedNormal, cameraPosition);

        _pixelDrawer.Draw(x, y, pixelColor);
    }

    private int CalculateMinX(Point point0, Point point1, Point point2)
    {
        return (int)Math.Max(0, Math.Floor(Math.Min(point0.X, Math.Min(point1.X, point2.X))));
    }

    private int CalculateMaxX(Point point0, Point point1, Point point2)
    {
        return (int)Math.Min(_width - 1, Math.Ceiling(Math.Max(point0.X, Math.Max(point1.X, point2.X))));
    }

    private int CalculateMinY(Point point0, Point point1, Point point2)
    {
        return (int)Math.Max(0, Math.Floor(Math.Min(point0.Y, Math.Min(point1.Y, point2.Y))));
    }

    private int CalculateMaxY(Point point0, Point point1, Point point2)
    {
        return (int)Math.Min(_height - 1, Math.Ceiling(Math.Max(point0.Y, Math.Max(point1.Y, point2.Y))));
    }
}