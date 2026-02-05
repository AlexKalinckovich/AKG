namespace AKG.Core.Parser.Numeric;


using System.Globalization;

public class IntegerStringParser
{
    public int ParseAndAdjustIndexFromString(string indexString)
    {
        int parsedIndex = int.Parse(indexString, NumberStyles.Integer, CultureInfo.InvariantCulture);
        
        return AdjustParsedIndex(parsedIndex);
    }

    private int AdjustParsedIndex(int parsedIndex)
    {
        if (parsedIndex > 0)
        {
            return parsedIndex - 1;
        }
        
        return parsedIndex;
    }

    public bool CanParseStringToInteger(string inputString)
    {
        return int.TryParse(inputString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
    }
}