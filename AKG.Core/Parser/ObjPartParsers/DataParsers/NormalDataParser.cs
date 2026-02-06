using System.Numerics;
using AKG.Core.Parser.Numeric;

namespace AKG.Core.Parser.ObjPartParsers.DataParsers;

public class NormalDataParser
{
    private readonly FloatStringParser _floatStringParser = new();

    public Vector3 ParseNormalDataString(string dataString)
    {
        string[] dataParts = SplitDataString(dataString);
        
        if (HasEnoughNormalDataParts(dataParts))
        {
            return CreateVector3FromDataParts(dataParts);
        }
        
        return new Vector3(0, 0, 0);
    }

    private string[] SplitDataString(string dataString)
    {
        return dataString.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
    }

    private bool HasEnoughNormalDataParts(string[] dataParts)
    {
        return dataParts.Length >= 3;
    }

    private Vector3 CreateVector3FromDataParts(string[] dataParts)
    {
        float x = _floatStringParser.ParseFloatFromString(dataParts[0]);
        
        float y = _floatStringParser.ParseFloatFromString(dataParts[1]);
        
        float z = _floatStringParser.ParseFloatFromString(dataParts[2]);

        return new Vector3(x, y, z);
    }
}