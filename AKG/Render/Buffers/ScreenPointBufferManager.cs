// ================ ScreenPointBufferManager.cs ================

using System.Buffers;
using System.Windows;

namespace AKG.Render.Buffers;

public class ScreenPointBufferManager : IDisposable
{
    private Point[] _pointBuffer = Array.Empty<Point>();
    private int _currentVertexCount = 0;

    public Point[] GetOrCreateBuffer(int requiredCapacity)
    {
        EnsureBufferCapacity(requiredCapacity);
        
        _currentVertexCount = requiredCapacity;
        
        return _pointBuffer;
    }

    public Point[] GetBufferArray()
    {
        return _pointBuffer;
    }

    public int CurrentBufferSize => _currentVertexCount;

    private void EnsureBufferCapacity(int requiredCapacity)
    {
        if (_pointBuffer.Length >= requiredCapacity)
        {
            return;
        }

        int newCapacity = CalculateNewCapacity(requiredCapacity);
        Point[] newBuffer = ArrayPool<Point>.Shared.Rent(newCapacity);
        
        if (_pointBuffer.Length > 0)
        {
            Array.Copy(_pointBuffer, newBuffer, _currentVertexCount);
            ArrayPool<Point>.Shared.Return(_pointBuffer);
        }

        _pointBuffer = newBuffer;
    }

    private int CalculateNewCapacity(int requiredCapacity)
    {
        if (_pointBuffer.Length == 0)
        {
            return requiredCapacity;
        }

        int newCapacity = _pointBuffer.Length;
        while (newCapacity < requiredCapacity)
        {
            newCapacity *= 2;
        }

        return newCapacity;
    }

    public void Dispose()
    {
        if (_pointBuffer.Length > 0)
        {
            ArrayPool<Point>.Shared.Return(_pointBuffer);
            _pointBuffer = Array.Empty<Point>();
        }
        _currentVertexCount = 0;
    }
}