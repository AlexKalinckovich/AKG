namespace AKG.FileHelper;

public readonly struct FileChunk(byte[] buffer, int count)
{
    public byte[] Buffer { get; } = buffer;
    public int Count { get; } = count;
}