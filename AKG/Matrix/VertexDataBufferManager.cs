using AKG.Model.Vertex;

namespace AKG.Matrix;

public sealed class VertexDataBufferManager
{
    private VertexData[] _buffer = [];
    public int CurrentBufferSize { get; private set; }

    public VertexData[] GetOrCreateBuffer(int requiredSize)
    {
        if (_buffer.Length < requiredSize)
        {
            _buffer = new VertexData[requiredSize];
        }

        CurrentBufferSize = requiredSize;
        return _buffer;
    }
}
