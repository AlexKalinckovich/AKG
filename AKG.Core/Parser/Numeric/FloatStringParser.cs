
using System.Globalization;

namespace AKG.Core.Parser.Numeric;

public class FloatStringParser
{
    public float ParseFloatFromString(string inputString)
    {
        return float.Parse(inputString, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    public bool CanParseStringToFloat(string inputString)
    {
        return float.TryParse(inputString, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
    }
}