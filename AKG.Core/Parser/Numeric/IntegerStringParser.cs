namespace AKG.Core.Parser.Numeric;


using System.Globalization;

public class IntegerStringParser
{
    public void ParseAndAdjustIndexFromString(string indexString, out int parsedIndex)
    {
        int.TryParse(indexString, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedIndex);
        
        parsedIndex = AdjustParsedIndex(parsedIndex);
    }

    private int AdjustParsedIndex(int parsedIndex)
    {
        if (parsedIndex > 0)
        {
            return parsedIndex - 1;
        }
        
        return parsedIndex;
    }
}