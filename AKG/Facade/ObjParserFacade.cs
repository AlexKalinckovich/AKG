using ACG.Model;
using AKG.Core.Model;
using AKG.Core.Parser;
using AKG.FileHelper;

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

    public async Task ParseObjModelFromFileAsync(Action? onComplete)
    {
        CurrentObjModel.Clear();
        
        int totalChunks = 0;
        int processedChunks = 0;
        
        // Загружаем и парсим файл по чанкам
        var chunks = _fileLoader.LoadFileByChunksAsync();
        var model = await _parser.ParseAsync(chunks);
        
        CurrentObjModel = model;
        
        onComplete?.Invoke();
    }
    
    // Метод для отображения прогресса
    public async Task ParseWithProgressAsync(IProgress<(int, int)> progress)
    {
        CurrentObjModel.Clear();
        
        var chunks = _fileLoader.LoadFileByChunksAsync();
        var model = new ObjModel();
        int totalChunks = 0;
        int processedChunks = 0;
        
        await foreach (var chunk in chunks)
        {
            totalChunks++;
        }
        
        // Сбрасываем итератор
        chunks = _fileLoader.LoadFileByChunksAsync();
        
        await foreach (var chunk in chunks)
        {
            var partialData = new ChunkProcessor().ProcessChunk(chunk);
            model.MergeWith(partialData);
            processedChunks++;
            
            progress.Report((processedChunks, totalChunks));
        }
        
        CurrentObjModel = model;
    }
}