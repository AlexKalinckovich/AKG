using AKG.Core.Model;

namespace AKG.Core.Parser;

public class ObjParser
{
    private readonly ChunkProcessor _processor;
    
    public ObjParser()
    {
        _processor = new ChunkProcessor();
    }
    
    public async Task<ObjModel> ParseAsync(IAsyncEnumerable<string[]> chunks)
    {
        ObjModel model = new ObjModel();
        
        await foreach (var chunk in chunks)
        {
            var partialData = _processor.ProcessChunk(chunk);
            model.MergeWith(partialData);
        }
        
        return model;
    }
}