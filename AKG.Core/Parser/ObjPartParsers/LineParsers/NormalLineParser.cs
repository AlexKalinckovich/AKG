using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;
using AKG.Core.Parser.ObjPartParsers.DataParsers;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;


using System.Numerics;

public class NormalLineParser : ILineTypeParser
{
    private readonly NormalDataParser _normalDataParser = new();

    public LineType SupportedLineType => LineType.Normal;

    public void ParseLine(string line, PartialModelData partialModelData)
    {
        string dataString = ExtractDataSubstringFromLine(line);
        
        Vector3 normal = _normalDataParser.ParseNormalDataString(dataString);
        
        partialModelData.Normals.Add(normal);
    }

    private string ExtractDataSubstringFromLine(string line)
    {
        return line.Substring(2).TrimStart();
    }
}