namespace AKG.Render.Rasterization;


public sealed class ZBufferManager : IDisposable
{
    private readonly double[,] _zBuffer;
    private readonly int _width;
    private readonly int _height;

    public ZBufferManager(int width, int height)
    {
        _width = width;
        _height = height;
        _zBuffer = new double[width, height];
        Clear();
    }

    public void Clear()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _zBuffer[x, y] = double.MaxValue;
            }
        }
    }

    public bool ShouldUpdatePixel(int x, int y, float depth)
    {
        return depth < _zBuffer[x, y];
    }

    public void UpdateDepth(int x, int y, float depth)
    {
        _zBuffer[x, y] = depth;
    }

    public void Dispose()
    { }
}