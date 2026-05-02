using System;
using System.Numerics;
using AKG.Render.Rasterization;

namespace AKG.Render.Texture;

public readonly struct Texture2D
{
    private readonly Vector3[,] _pixels;
    public readonly int Width;
    public readonly int Height;

    public Texture2D(int width, int height, Vector3[,] pixelData)
    {
        Width = width;
        Height = height;
        _pixels = pixelData;
    }

    private Vector3 GetPixel(int x, int y)
    {
        bool isOutOfBounds = x < 0 || x >= Width || y < 0 || y >= Height;
        
        return isOutOfBounds ? Vector3.Zero : _pixels[x, y];
    }

    public Vector3 Sample(float u, float v)
    {
        int x = ClampCoordinate(u, Width);
        int y = ClampCoordinate(v, Height);
        return GetPixel(x, y);
    }

    public Vector3 SampleBilinear(float u, float v)
    {
        
        v = 1.0f - v;
        float fx = u * Width - 0.5f;
        float fy = v * Height - 0.5f;
    
        int x0 = (int)Math.Floor(fx);
        int y0 = (int)Math.Floor(fy);
        int x1 = x0 + 1;
        int y1 = y0 + 1;
    
        float dx = fx - x0;
        float dy = fy - y0;
        
    
        x0 = Math.Clamp(x0, 0, Width - 1);
        x1 = Math.Clamp(x1, 0, Width - 1);
        y0 = Math.Clamp(y0, 0, Height - 1);
        y1 = Math.Clamp(y1, 0, Height - 1);
        
    
        Vector3 c00 = GetPixel(x0, y0);
        Vector3 c01 = GetPixel(x0, y1);
        Vector3 c10 = GetPixel(x1, y0);
        Vector3 c11 = GetPixel(x1, y1);
    
        Vector3 c0 = Vector3.Lerp(c00, c10, dx);
        Vector3 c1 = Vector3.Lerp(c01, c11, dx);

        Vector3 result = Vector3.Lerp(c0, c1, dy);
        
        return result;
    }
    
    private static int ClampCoordinate(float coordinate, int dimension)
    {
        int index = (int)(coordinate * dimension);
        
        return Math.Clamp(index, 0, dimension - 1);
    }
}