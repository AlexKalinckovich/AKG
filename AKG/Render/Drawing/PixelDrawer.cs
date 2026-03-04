using AKG.Render.Renderers;

namespace AKG.Render.Drawing;


public sealed class PixelDrawer
{
    private readonly BitmapRenderer _bitmapRenderer;
    private readonly int _width;
    private readonly int _height;

    public PixelDrawer(BitmapRenderer bitmapRenderer, int width, int height)
    {
        _bitmapRenderer = bitmapRenderer;
        _width = width;
        _height = height;
    }

    public void Draw(int x, int y, uint color)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _bitmapRenderer.SetPixel(x, y, color);
        }
    }
}