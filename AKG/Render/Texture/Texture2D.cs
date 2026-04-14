using System;
using System.Numerics;

namespace AKG.Render.Texture;

public readonly struct Texture2D
{
    private readonly Vector3[,] _pixels;
    private readonly int _width;
    private readonly int _height;

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

    public Vector3 Sample(float u, float v)
    {
        int x = ClampCoordinate(u, _width);
        int y = ClampCoordinate(v, _height);
        return GetPixel(x, y);
    }

    private static int ClampCoordinate(float coordinate, int dimension)
    {
        int index = (int)(coordinate * dimension);
        
        return Math.Clamp(index, 0, dimension - 1);
    }
}