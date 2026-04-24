using System.Windows;

namespace AKG.Render.Rasterization;

public sealed class ZBufferManager : IDisposable
{
    private readonly float[,] _zBuffer;
    private readonly int _width;
    private readonly int _height;
    private readonly Lock _lock = new Lock();

    public ZBufferManager(int width, int height)
    {
        _width = width;
        _height = height;
        _zBuffer = new float[width, height];
        Clear();
    }

    public void Clear()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _zBuffer[x, y] = 1.0f;
            }
        }
    }

    public bool TryUpdateDepth(Point pixel, float depth)
    {
        int x = (int)pixel.X;
        int y = (int)pixel.Y;

        lock (_lock)
        {
            if (depth < _zBuffer[x, y])
            {
                _zBuffer[x, y] = depth;
                return true;
            }
            return false;
        }
    }

    public bool ShouldUpdatePixel(Point pixel, float depth)
    {
        int x = (int)pixel.X;
        int y = (int)pixel.Y;

        return depth < _zBuffer[x, y];
    }

    public void UpdateDepth(Point pixel, float depth)
    {
        int x = (int)pixel.X;
        int y = (int)pixel.Y;

        _zBuffer[x, y] = depth;
    }

    public void Dispose()
    {
    }
}