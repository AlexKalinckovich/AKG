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
        Triangle triangle = new Triangle(vertex0, vertex1, vertex2);
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
                ProcessPixel(x, y, triangle, cameraPosition);
            }
        }
    }

    private void ProcessPixel(int x, int y, Triangle triangle, Vector3 cameraPosition)
    {
        if (_barycentricCalculator.IsPointInTriangle(x, y, triangle))
        {
            RasterizePointInTriangle(x, y, triangle, cameraPosition);
        }
    }

    private void RasterizePointInTriangle(int x, int y, Triangle triangle, Vector3 cameraPosition)
    {
        TriangleWeight triangleWeight = _barycentricCalculator.ComputeBarycentricCoordinates(x, y, triangle);

        float depth = triangleWeight.Weight0 * triangle.Vertex0.Depth +
                      triangleWeight.Weight1 * triangle.Vertex1.Depth +
                      triangleWeight.Weight2 * triangle.Vertex2.Depth;

        if (_zBufferManager.ShouldUpdatePixel(x, y, depth))
        {
            _zBufferManager.UpdateDepth(x, y, depth);
            
            Vector3 interpolatedNormal = CalculateInterpolatedNormal(triangle, triangleWeight);

            Vector3 interpolatedWorldPos = CalculateInterpolatedWorldPos(triangle, triangleWeight);

            UpdatePixelLightColor(x, y, interpolatedNormal, interpolatedWorldPos, cameraPosition);
        }
    }

    private void UpdatePixelLightColor(int x, int y, Vector3 interpolatedNormal, Vector3 interpolatedWorldPos, Vector3 cameraPosition)
    {
        
        uint pixelColor = _lightingCalculator.CalculatePixelColor(interpolatedWorldPos, interpolatedNormal, cameraPosition);

        _pixelDrawer.Draw(x, y, pixelColor);
    }

    private static Vector3 CalculateInterpolatedWorldPos(Triangle triangle, TriangleWeight triangleWeight)
    {
        Vector3 interpolatedWorldPos = triangleWeight.Weight0 * triangle.Vertex0.WorldPosition +
                                       triangleWeight.Weight1 * triangle.Vertex1.WorldPosition +
                                       triangleWeight.Weight2 * triangle.Vertex2.WorldPosition;
        return interpolatedWorldPos;
    }

    private static Vector3 CalculateInterpolatedNormal(Triangle triangle, TriangleWeight triangleWeight)
    {
        Vector3 interpolatedNormal = triangleWeight.Weight0 * triangle.Vertex0.Normal +
                                     triangleWeight.Weight1 * triangle.Vertex1.Normal +
                                     triangleWeight.Weight2 * triangle.Vertex2.Normal;
        return interpolatedNormal;
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