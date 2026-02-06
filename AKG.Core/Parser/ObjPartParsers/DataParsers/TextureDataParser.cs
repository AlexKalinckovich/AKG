using System.Numerics;
using AKG.Core.Parser.Numeric;

namespace AKG.Core.Parser.ObjPartParsers.DataParsers;

public class TextureDataParser
{
    private readonly FloatStringParser _floatStringParser = new();

    public Vector2 ParseTextureDataString(string dataString)
    {
        string[] dataParts = SplitDataString(dataString);
        
        if (HasEnoughTextureDataParts(dataParts))
        {
            return CreateVector2FromDataParts(dataParts);
        }
        
        return new Vector2(0, 0);
    }

    private string[] SplitDataString(string dataString)
    {
        return dataString.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
    }

    private bool HasEnoughTextureDataParts(string[] dataParts)
    {
        return dataParts.Length >= 2;
    }

    private Vector2 CreateVector2FromDataParts(string[] dataParts)
    {
        float u = _floatStringParser.ParseFloatFromString(dataParts[0]);
        
        float v = _floatStringParser.ParseFloatFromString(dataParts[1]);

        return new Vector2(u, v);
    }
}