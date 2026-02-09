using AKG.Model;

namespace AKG.Matrix;

public class VertexDataBufferManager : IDisposable
{
    private VertexData[]? _buffer;
    public int CurrentBufferSize { get; private set; }

    public VertexData[] GetOrCreateBuffer(int requiredSize)
    {
        if (_buffer == null || _buffer.Length < requiredSize)
        {
            _buffer = new VertexData[requiredSize];
        }

        CurrentBufferSize = requiredSize;
        return _buffer;
    }

    public VertexData[] GetBufferArray()
    {
        return _buffer!;
    }

    public void Dispose()
    {
        _buffer = null;
    }
}
