using ACG.Model;
using AKG.FileHelper;
using AKG.Parser;

namespace AKG.Facade;

public class ObjParserFacade
{
    public ObjModel CurrentObjModel { get; private set; }
    private readonly ObjFileLoader _fileLoader;
    private readonly ObjParser _parser;
    
    public ObjParserFacade()
    {
        CurrentObjModel = new ObjModel();
        _parser = new ObjParser();
        _fileLoader = new ObjFileLoader();
    }

    public async Task ParseObjModelFromFileAsync(Action onProgress, Action onComplete)
    {
        CurrentObjModel.Clear();
        
        FileChunk fileChunk = await _fileLoader.LoadEntireFileAsync();

        CurrentObjModel = _parser.Parse(fileChunk);
        
        onComplete?.Invoke();
    }
}