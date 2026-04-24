using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Drawing;
using AKG.Render.Lighting;
using AKG.Render.Renderers;
using AKG.Render.States;
using AKG.Render.Texture;

namespace AKG.Render.Rasterization;

public sealed class TriangleRasterizer
{
    private readonly int _bitmapPixelWidth;
    private readonly int _bitmapPixelHeight;
    private readonly ZBufferManager _zBufferManager;
    private readonly LightingCalculator _lightingCalculator;
    private readonly PixelDrawer _pixelDrawer;
    private readonly CameraState _cameraState;

    public TriangleRasterizer(BitmapRenderer bitmapRenderer, CameraState cameraState)
    {
        _bitmapPixelWidth = bitmapRenderer.Width;
        _bitmapPixelHeight = bitmapRenderer.Height;

        _lightingCalculator = LightCalculatorBuilder.CreateLightingCalculatorWithTexture(
            RenderTextureMaps.CreateDefaultRenderMaps());

        _pixelDrawer = new PixelDrawer(bitmapRenderer, _bitmapPixelWidth, _bitmapPixelHeight);
        _zBufferManager = new ZBufferManager(_bitmapPixelWidth, _bitmapPixelHeight);
        _cameraState = cameraState;
    }

    public TriangleRasterizer(
        BitmapRenderer bitmapRenderer,
        CameraState cameraState,
        RenderTextureMaps renderTextureMaps)
    {
        _bitmapPixelWidth = bitmapRenderer.Width;
        _bitmapPixelHeight = bitmapRenderer.Height;

        _lightingCalculator = LightCalculatorBuilder.CreateLightingCalculatorWithTexture(renderTextureMaps);
        _pixelDrawer = new PixelDrawer(bitmapRenderer, _bitmapPixelWidth, _bitmapPixelHeight);
        _zBufferManager = new ZBufferManager(_bitmapPixelWidth, _bitmapPixelHeight);
        _cameraState = cameraState;
    }

    public void ClearZBuffer()
    {
        _zBufferManager.Clear();
    }

    public void Rasterize(Triangle triangle)
    {
        Point point0 = triangle.Vertex0.ScreenPoint;
        Point point1 = triangle.Vertex1.ScreenPoint;
        Point point2 = triangle.Vertex2.ScreenPoint;

        float area = (float)((point1.X - point0.X) * (point2.Y - point0.Y) - 
                              (point1.Y - point0.Y) * (point2.X - point0.X));
        if (area >= 0) return;

        int minX = CalculateMinX(point0, point1, point2);
        int maxX = CalculateMaxX(point0, point1, point2);
        int minY = CalculateMinY(point0, point1, point2);
        int maxY = CalculateMaxY(point0, point1, point2);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Point point = new Point(x, y);
                ProcessPixel(point, triangle);
            }
        }
    }

    private void ProcessPixel(Point point, Triangle triangle)
    {
        if (BarycentricCalculator.IsPointInTriangle(point, triangle))
        {
            RasterizePointInTriangle(point, triangle);
        }
    }

    private void RasterizePointInTriangle(Point pointInTriangle, Triangle triangle)
    {
        TriangleWeight screenWeights = BarycentricCalculator.ComputeBarycentricCoordinates(
            pointInTriangle, triangle);

        float z0 = 1 / triangle.Vertex0.ClipPosition.W;
        float z1 = 1 / triangle.Vertex1.ClipPosition.W;
        float z2 = 1 / triangle.Vertex2.ClipPosition.W;

        float depth = screenWeights.Weight0 * z0 + 
                      screenWeights.Weight1 * z1 + 
                      screenWeights.Weight2 * z2;

        if (_zBufferManager.ShouldUpdatePixel(pointInTriangle, depth))
        {
            _zBufferManager.UpdateDepth(pointInTriangle, depth);

            Vector2 uv = BarycentricCalculator.ComputePerspectiveCorrectUv(triangle, screenWeights);

            Vector3 worldPos = CalculateInterpolatedWorldPosCorrected(triangle, screenWeights);

            uint pixelColor = _lightingCalculator.CalculatePixelColor(worldPos, _cameraState.TargetPosition, uv);
            UpdatePixelLightColor(pointInTriangle, pixelColor);
        }
    }

    private Vector3 CalculateInterpolatedWorldPosCorrected(Triangle triangle, TriangleWeight screenWeights)
    {
        float invW0 = 1.0f / triangle.Vertex0.ClipPosition.W;
        float invW1 = 1.0f / triangle.Vertex1.ClipPosition.W;
        float invW2 = 1.0f / triangle.Vertex2.ClipPosition.W;

        float invDepth = screenWeights.Weight0 * invW0 +
                         screenWeights.Weight1 * invW1 +
                         screenWeights.Weight2 * invW2;

        Vector3 world0 = triangle.Vertex0.WorldPosition;
        Vector3 world1 = triangle.Vertex1.WorldPosition;
        Vector3 world2 = triangle.Vertex2.WorldPosition;

        Vector3 worldPos = (screenWeights.Weight0 * world0 * invW0 +
                            screenWeights.Weight1 * world1 * invW1 +
                            screenWeights.Weight2 * world2 * invW2) / invDepth;

        return worldPos;
    }

    private void UpdatePixelLightColor(Point pixel, uint pixelColor)
    {
        int x = (int)pixel.X;
        int y = (int)pixel.Y;

        _pixelDrawer.Draw(x, y, pixelColor);
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