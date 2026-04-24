using AKG.Render.Renderers;

namespace AKG.Render.Drawing;


public sealed class PixelDrawer
{
    
    private readonly BitmapRenderer _bitmapRenderer;

    public PixelDrawer(BitmapRenderer bitmapRenderer, int width, int height)
    {
        _bitmapRenderer = bitmapRenderer;
    }

    public void Draw(int x, int y, uint color)
    {
        _bitmapRenderer.SetPixel(x, y, color);
    }
}