using AKG.Core.Model;

namespace AKG.Core.Parser;

public class ChunkProcessor
{
    private LineParser _lineParser;

    public ChunkProcessor()
    {
        _lineParser = new LineParser();
    }

    public ParticalModelData ProcessChunk(string[] lines)
    {
        return _lineParser.ParseLines(lines);
    }
}