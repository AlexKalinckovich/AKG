using AKG.Core.Model;
using AKG.Core.Parser;
using AKG.FileHelper;

namespace AKG.Facade;

public class ObjParserFacade
{
    private readonly ObjFileLoader _fileLoader;
    private readonly ObjParser _parser;
    
    public ObjParserFacade()
    {
        _parser = new ObjParser();
        _fileLoader = new ObjFileLoader();
    }

    public async Task<ObjModel> ParseObjModelFromFileAsync()
    {
        IAsyncEnumerable<string[]> chunks = _fileLoader.LoadFileByChunksAsync();
        
        return await _parser.ParseAsync(chunks);
    }
    
}