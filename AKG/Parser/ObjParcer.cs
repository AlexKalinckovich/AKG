using System.Buffers;
using ACG.Model;
using ACG.Parser;
using AKG.FileHelper;

namespace AKG.Parser;

public class ObjParser
{
    private readonly ChunkProcessor _processor;
    
    public ObjParser()
    {
        _processor = new ChunkProcessor();
    }
    
    public ObjModel Parse(FileChunk chunk)
    {
        ObjModel model = _processor.Process(chunk.Buffer, chunk.Count);
        //ArrayPool<byte>.Shared.Return(chunk.Buffer);
        return model;
    }
}