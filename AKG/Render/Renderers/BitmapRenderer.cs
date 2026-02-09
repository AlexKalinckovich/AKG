// ================ BitmapRenderer.cs ================

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using AKG.Render.Constants;

namespace AKG.Render.Renderers;

public unsafe class BitmapRenderer
{
    private readonly WriteableBitmap _bitmap;
    private readonly int _width;
    private readonly int _height;
    private readonly int _stride;
    private readonly byte* _backBuffer;

    public BitmapRenderer(WriteableBitmap bitmap)
    {
        _bitmap = bitmap;
        _width = bitmap.PixelWidth;
        _height = bitmap.PixelHeight;
        _stride = bitmap.BackBufferStride;

        _bitmap.Lock();
        try
        {
            _backBuffer = (byte*)_bitmap.BackBuffer;
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    public void BeginDrawing()
    {
        _bitmap.Lock();
    }

    public void EndDrawing()
    {
        Int32Rect dirtyRect = new Int32Rect(0, 0, _width, _height);
        
        _bitmap.AddDirtyRect(dirtyRect);
        
        _bitmap.Unlock();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearBitmap()
    {
        if (_height > RenderConstants.ParallelProcessingThreshold)
        {
            ClearBitmapInParallel();
        }
        else
        {
            ClearBitmapSequentially();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearBitmapInParallel()
    {
        Parallel.For(0, _height, y =>
        {
            byte* rowPointer = _backBuffer + y * _stride;
            ClearRowUsingUInt(rowPointer);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearBitmapSequentially()
    {
        for (int y = 0; y < _height; y++)
        {
            byte* rowPointer = _backBuffer + y * _stride;
            ClearRowUsingBytes(rowPointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearRowUsingUInt(byte* rowPointer)
    {
        uint* uintPointer = (uint*)rowPointer;
        for (int x = 0; x < _width; x++)
        {
            uintPointer[x] = RenderConstants.BlackColor;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearRowUsingBytes(byte* rowPointer)
    {
        for (int x = 0; x < _width; x++)
        {
            int offset = x * RenderConstants.BytesPerPixel;
            rowPointer[offset] = 0;
            rowPointer[offset + 1] = 0;
            rowPointer[offset + 2] = 0;
            rowPointer[offset + 3] = 255;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawLine(Point point1, Point point2)
    {
        DrawLineUnsafe((int)point1.X, (int)point1.Y, (int)point2.X, (int)point2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawLineUnsafe(int x0, int y0, int x1, int y1)
    {
        int deltaX = Math.Abs(x1 - x0);
        int deltaY = Math.Abs(y1 - y0);

        int stepX = GetStepDirection(x0, x1);
        int stepY = GetStepDirection(y0, y1);

        int error = deltaX - deltaY;
        int maximumX = _width - 1;
        int maximumY = _height - 1;

        while (true)
        {
            DrawPixelIfWithinBounds(x0, y0, maximumX, maximumY);

            if (IsLineDrawingComplete(x0, y0, x1, y1))
            {
                break;
            }

            UpdateLinePosition(ref x0, ref y0, ref error, deltaX, deltaY, stepX, stepY);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetStepDirection(int start, int end)
    {
        return start < end ? 1 : -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawPixelIfWithinBounds(int x, int y, int maxX, int maxY)
    {
        if (x >= 0 && x <= maxX && y >= 0 && y <= maxY)
        {
            int offset = y * _stride + x * RenderConstants.BytesPerPixel;
            *(uint*)(_backBuffer + offset) = RenderConstants.WhiteColor;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsLineDrawingComplete(int currentX, int currentY, int targetX, int targetY)
    {
        return currentX == targetX && currentY == targetY;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateLinePosition(ref int x, ref int y, ref int error, int deltaX, int deltaY, int stepX, int stepY)
    {
        int doubledError = 2 * error;

        if (doubledError > -deltaY)
        {
            error -= deltaY;
            x += stepX;
        }

        if (doubledError < deltaX)
        {
            error += deltaX;
            y += stepY;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPixel(int x, int y, uint color)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            int offset = y * _stride + x * RenderConstants.BytesPerPixel;
            *(uint*)(_backBuffer + offset) = color;
        }
    }
}