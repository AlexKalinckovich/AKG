using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;
using AKG.Core.Parser.ObjPartParsers.LineParsers;

namespace AKG.Core.Parser.ObjPartParsers;

public class LineParserFactory
{
    private readonly Dictionary<LineType, ILineTypeParser> _parserDictionary = new();
    
    
    public LineParserFactory()
    {
        InitializeParserDictionary();
    }

    private void InitializeParserDictionary()
    {
        RegisterParser(new VertexLineParser());
        RegisterParser(new TextureLineParser());
        RegisterParser(new NormalLineParser());
        RegisterParser(new FaceLineTypeParser());
        RegisterParser(new IgnoreLineParser());
    }

    private void RegisterParser(ILineTypeParser lineTypeParser)
    {
        _parserDictionary[lineTypeParser.SupportedLineType] = lineTypeParser;
    }

    public ILineTypeParser GetParserForLineType(LineType lineType)
    {
        return _parserDictionary.TryGetValue(lineType, out var parser) ? 
            parser : throw new ArgumentException($"No parsers for that lineType: {lineType.GetType().FullName}");
    }
}