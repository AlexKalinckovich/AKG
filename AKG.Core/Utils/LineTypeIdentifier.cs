namespace AKG.Core.Parser;

public class LineTypeIdentifier
{
    public LineType IdentifyLineType(string line)
    {
        if (IsLineTooShortForParsing(line))
        {
            return LineType.Ignore;
        }

        return IdentifyLineTypeOfTrimmedLine(line);
    }

    private bool IsLineTooShortForParsing(string line)
    {
        return string.IsNullOrWhiteSpace(line) || line.Length < 2;
    }
    
    private LineType IdentifyLineTypeOfTrimmedLine(string line)
    {
        string trimmedLine = line.TrimStart();
        
        if (trimmedLine.Length < 2)
        {
            return LineType.Ignore;
        }

        return DetermineSpecificLineType(trimmedLine);
    }

    
    private LineType DetermineSpecificLineType(string trimmedLine)
    {
        char firstCharacter = trimmedLine[0];
        
        char secondCharacter = trimmedLine.Length > 1 ? trimmedLine[1] : ' ';

        return IdentifyTypeByFirstTwoChars(trimmedLine, firstCharacter, secondCharacter);
    }

    private LineType IdentifyTypeByFirstTwoChars(string trimmedLine, char firstCharacter, char secondCharacter)
    {
        return firstCharacter switch
        {
            'v' => DetermineVertexLineType(secondCharacter, trimmedLine),
            'f' => LineType.Face, 
             _  => LineType.Ignore
        };
    }

    private LineType DetermineVertexLineType(char secondCharacter, string trimmedLine)
    {
        if (secondCharacter == ' ' || secondCharacter == '\t')
        {
            return LineType.Vertex;
        }

        return DetermineComplexVertexType(secondCharacter, trimmedLine);
    }

    private static LineType DetermineComplexVertexType(char secondCharacter, string trimmedLine)
    {
        if (trimmedLine.Length > 2 && trimmedLine[2] == ' ')
        {
            return IdentifyVertexLineType(secondCharacter);
        }

        return LineType.Ignore;
    }

    private static LineType IdentifyVertexLineType(char secondCharacter)
    {
        switch (secondCharacter)
        {
            case 't': return LineType.Texture; // vt
            
            case 'n': return LineType.Normal; // vn
            
            default:  return LineType.Ignore; // trash
        }
    }
}