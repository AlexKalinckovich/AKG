using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;
using AKG.Core.Parser.ObjPartParsers.DataParsers;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;


using System.Numerics;

public class TextureLineParser : ILineTypeParser
{
    private readonly TextureDataParser _textureDataParser = new();

    public LineType SupportedLineType => LineType.Texture;

    public void ParseLine(string line, PartialModelData partialModelData)
    {
        string dataString = ExtractDataSubstringFromLine(line);
        
        Vector2 textureCoord = _textureDataParser.ParseTextureDataString(dataString);
        
        partialModelData.TextureCoords.Add(textureCoord);
    }

    private string ExtractDataSubstringFromLine(string line)
    {
        return line.Substring(2).TrimStart();
    }
}