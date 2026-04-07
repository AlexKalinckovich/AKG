using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Drawing;
using AKG.Render.Lighting;
using AKG.Render.Renderers;

namespace AKG.Render.Rasterization;

public sealed class TriangleRasterizer
{
    private readonly int _bitmapPixelWidth;
    private readonly int _bitmapPixelHeight;
    private readonly ZBufferManager _zBufferManager;
    private readonly LightingCalculator _lightingCalculator;
    private readonly PixelDrawer _pixelDrawer;

    private readonly RenderTextureMaps _renderTextureMaps;


    public TriangleRasterizer(BitmapRenderer bitmapRenderer)
    {
        _bitmapPixelWidth = bitmapRenderer.Width;
        _bitmapPixelHeight = bitmapRenderer.Height;

        _lightingCalculator = LightCalculatorBuilder.CreateLightingCalculator();

        _pixelDrawer = new PixelDrawer(bitmapRenderer, _bitmapPixelWidth, _bitmapPixelHeight);

        _zBufferManager = new ZBufferManager(_bitmapPixelWidth, _bitmapPixelHeight);

        _renderTextureMaps = RenderTextureMaps.CreateDefaultRenderMaps();
    }

    public TriangleRasterizer(
        BitmapRenderer bitmapRenderer,
        RenderTextureMaps renderTextureMaps)
    {
        _bitmapPixelWidth = bitmapRenderer.Width;
        _bitmapPixelHeight = bitmapRenderer.Height;
        
        _lightingCalculator = LightCalculatorBuilder.CreateLightingCalculator();

        _pixelDrawer = new PixelDrawer(bitmapRenderer, _bitmapPixelWidth, _bitmapPixelHeight);

        _zBufferManager = new ZBufferManager(_bitmapPixelWidth, _bitmapPixelHeight);

        _renderTextureMaps = renderTextureMaps;
    }
    
    public void ClearZBuffer()
    {
        _zBufferManager.Clear();
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
                Point point = new Point(x, y);
                ProcessPixel(point, triangle, cameraPosition);
            }
        }
    }

    private void ProcessPixel(Point point, Triangle triangle, Vector3 cameraPosition)
    {
        if (BarycentricCalculator.IsPointInTriangle(point, triangle))
        {
            RasterizePointInTriangle(point, triangle, cameraPosition);
        }
    }

    private void RasterizePointInTriangle(Point pointInTriangle, Triangle triangle, Vector3 cameraPosition)
    {
        TriangleWeight triangleWeight = BarycentricCalculator.ComputeBarycentricCoordinates(
            pointInTriangle, triangle);

        float depth = triangleWeight.Weight0 * triangle.Vertex0.Depth +
                      triangleWeight.Weight1 * triangle.Vertex1.Depth +
                      triangleWeight.Weight2 * triangle.Vertex2.Depth;

        if (_zBufferManager.ShouldUpdatePixel(pointInTriangle, depth))
        {
            _zBufferManager.UpdateDepth(pointInTriangle, depth);

            Vector2 uv = BarycentricCalculator.ComputePerspectiveCorrectUv(triangle, triangleWeight);

            uint pixelColor = CalculatePixelColor(triangle, triangleWeight, cameraPosition, uv);

            UpdatePixelLightColor(pointInTriangle, pixelColor);
        }
    }

    private uint CalculatePixelColor(
        Triangle triangle,
        TriangleWeight triangleWeight,
        Vector3 cameraPosition,
        Vector2 uv)
    {
        Vector3 interpolatedWorldPos = CalculateInterpolatedWorldPos(triangle, triangleWeight);

        uint pixelColor = _lightingCalculator.CalculatePixelColor(interpolatedWorldPos, cameraPosition, uv, _renderTextureMaps);

        return pixelColor;
    }

    private void UpdatePixelLightColor(Point pixel, uint pixelColor)
    {
        int x = (int)pixel.X;
        int y = (int)pixel.Y;

        _pixelDrawer.Draw(x, y, pixelColor);
    }

    private static Vector3 CalculateInterpolatedWorldPos(Triangle triangle, TriangleWeight triangleWeight)
    {
        Vector3 interpolatedWorldPos = triangleWeight.Weight0 * triangle.Vertex0.WorldPosition +
                                       triangleWeight.Weight1 * triangle.Vertex1.WorldPosition +
                                       triangleWeight.Weight2 * triangle.Vertex2.WorldPosition;
        return interpolatedWorldPos;
    }
    
    private static int CalculateMinX(Point point0, Point point1, Point point2)
    {
        double minX = Math.Min(point0.X, Math.Min(point1.X, point2.X));
        double flooredMinX = Math.Floor(minX);
        double clampedMinX = Math.Max(0, flooredMinX);
        return (int)clampedMinX;
    }

    private int CalculateMaxX(Point point0, Point point1, Point point2)
    {
        double maxX = Math.Max(point0.X, Math.Max(point1.X, point2.X));
        double ceiledMaxX = Math.Ceiling(maxX);
        double clampedMaxX = Math.Min(_bitmapPixelWidth - 1, ceiledMaxX);
        return (int)clampedMaxX;
    }

    private static int CalculateMinY(Point point0, Point point1, Point point2)
    {
        double minY = Math.Min(point0.Y, Math.Min(point1.Y, point2.Y));
        double flooredMinY = Math.Floor(minY);
        double clampedMinY = Math.Max(0, flooredMinY);
        return (int)clampedMinY;
    }

    private int CalculateMaxY(Point point0, Point point1, Point point2)
    {
        double maxY = Math.Max(point0.Y, Math.Max(point1.Y, point2.Y));
        double ceiledMaxY = Math.Ceiling(maxY);
        double clampedMaxY = Math.Min(_bitmapPixelHeight - 1, ceiledMaxY);
        return (int)clampedMaxY;
    }
}