
namespace ACG.Parser;

public delegate bool ParseDelegate<T>(ReadOnlySpan<byte> source, out T value, out int bytesConsumed);
public class BaseParser
{
    protected static bool ParseValue<T>(ref ReadOnlySpan<byte> span, out T value, ParseDelegate<T> parserLogic)
    {
        int startIndex = GetFirstNotSpaceSymbolIndex(span);
            
        if (startIndex > 0)
        {
            span = span.Slice(startIndex);
        }

        bool result = parserLogic(span, out value, out int consumed);

        if (result)
        {
            span = span.Slice(consumed);
        }

        return result;
    }

    private static int GetFirstNotSpaceSymbolIndex(in ReadOnlySpan<byte> span)
    {
        const int spaceUnicode = 32;
        int index = 0;
        while (index < span.Length && span[index] <= spaceUnicode)
        {
            index++;
        }
        return index;
    }
}