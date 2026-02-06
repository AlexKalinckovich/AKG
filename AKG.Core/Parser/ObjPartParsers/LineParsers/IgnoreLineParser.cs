using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;

public class IgnoreLineParser : ILineTypeParser
{
    public LineType SupportedLineType => LineType.Ignore;
    
    public void ParseLine(string line, PartialModelData partialModelData)
    { }
}