using System;
using System.Numerics;

namespace AKG.Render.Texture;

public readonly struct Texture2D
{
    private readonly Vector3[,] _pixels;
    private readonly int _width;
    private readonly int _height;

    public int Width => _width;
    public int Height => _height;

    public Texture2D(int width, int height)
    {
        _width = width;
        _height = height;
        _pixels = new Vector3[width, height];
    }

    public Texture2D(int width, int height, Vector3[,] pixelData)
    {
        _width = width;
        _height = height;
        _pixels = pixelData;
    }

    public Vector3 GetPixel(int x, int y)
    {
        bool isOutOfBounds = x < 0 || x >= _width || y < 0 || y >= _height;
        if (isOutOfBounds)
        {
            return Vector3.Zero;
        }
        return _pixels[x, y];
    }

    public void SetPixel(int x, int y, Vector3 color)
    {
        bool isInBounds = x >= 0 && x < _width && y >= 0 && y < _height;
        if (isInBounds)
        {
            _pixels[x, y] = color;
        }
    }

    public Vector3 Sample(float u, float v)
    {
        int x = ClampCoordinate(u, _width);
        int y = ClampCoordinate(v, _height);
        return GetPixel(x, y);
    }

    public Vector3 SampleBilinear(float u, float v)
    {
        float fx = u * _width - 0.5f;
        float fy = v * _height - 0.5f;
        
        int x0 = (int)Math.Floor(fx);
        int y0 = (int)Math.Floor(fy);
        
        int x1 = x0 + 1;
        int y1 = y0 + 1;
        
        float dx = fx - x0;
        float dy = fy - y0;
        
        int clampedX0 = Math.Clamp(x0, 0, _width - 1);
        int clampedX1 = Math.Clamp(x1, 0, _width - 1);
        
        int clampedY0 = Math.Clamp(y0, 0, _height - 1);
        int clampedY1 = Math.Clamp(y1, 0, _height - 1);
        
        Vector3 c00 = GetPixel(clampedX0, clampedY0);
        Vector3 c01 = GetPixel(clampedX0, clampedY1);
        
        Vector3 c10 = GetPixel(clampedX1, clampedY0);
        Vector3 c11 = GetPixel(clampedX1, clampedY1);
        
        Vector3 bottomLerp = Vector3.Lerp(c00, c10, dx);
        Vector3 topLerp = Vector3.Lerp(c01, c11, dx);
        
        return Vector3.Lerp(bottomLerp, topLerp, dy);
    }

    private static int ClampCoordinate(float coordinate, int dimension)
    {
        int index = (int)(coordinate * dimension);
        
        return Math.Clamp(index, 0, dimension - 1);
    }
}