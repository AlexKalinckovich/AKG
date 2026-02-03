namespace AKG.FileHelper;

public class FileChunk
{
    public string[] Lines { get; }
    
    public FileChunk(string[] lines)
    {
        Lines = lines;
    }
}