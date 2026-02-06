using AKG.Core.Model;

namespace AKG.Core.Parser;

public class ObjParser
{
    
    private LineParser _lineParser;

    
    public ObjParser()
    {
        _lineParser = new LineParser();
    }
    
    public async Task<ObjModel> ParseAsync(IAsyncEnumerable<string[]> chunks)
    {
        ObjModel model = new ObjModel();
        
        await foreach (string[] chunk in chunks)
        {
            PartialModelData partialData = _lineParser.ParseLines(chunk);
            
            model.MergeWith(partialData);
        }
        
        return model;
    }
}