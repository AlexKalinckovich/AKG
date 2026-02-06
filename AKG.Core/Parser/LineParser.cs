// ================ LineParser.cs ================

using AKG.Core.Parser.ObjPartParsers;
using AKG.Core.Parser.ObjPartParsers.Abstraction;

namespace AKG.Core.Parser;

using Model;

public class LineParser
{
    private PartialModelData _partialModelData;
    private readonly LineTypeIdentifier _lineTypeIdentifier;
    private readonly LineParserFactory _lineParserFactory;

    public LineParser()
    {
        _partialModelData = new PartialModelData();
        _lineTypeIdentifier = new LineTypeIdentifier();
        _lineParserFactory = new LineParserFactory();
    }

    public PartialModelData ParseLines(string[] lines)
    {
        _partialModelData = new PartialModelData();
        
        ProcessEachLine(lines);
        
        return _partialModelData;
    }

    public PartialModelData GetPartialModel()
    {
        return _partialModelData;
    }

    private void ParseSingleLine(string line)
    {
        LineType lineType = _lineTypeIdentifier.IdentifyLineType(line);
        
        ProcessLineWithType(line, lineType);
    }

    private void ProcessEachLine(string[] lines)
    {
        foreach (string line in lines)
        {
            ParseSingleLine(line);
        }
    }

    private void ProcessLineWithType(string line, LineType lineType)
    {
        ILineTypeParser lineTypeParser = _lineParserFactory.GetParserForLineType(lineType);

        lineTypeParser.ParseLine(line, _partialModelData);
    }
}