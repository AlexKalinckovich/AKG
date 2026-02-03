using System.IO;
using Microsoft.Win32;

namespace AKG.FileHelper;

public class ObjFileLoader
{
    private const int DefaultLinesPerChunk = 256;
    
    public async IAsyncEnumerable<string[]> LoadFileByChunksAsync(int linesPerChunk = DefaultLinesPerChunk)
    {
        string? filePath = GetFilePathFromUser();
        if (filePath is null)
        {
            throw new OperationCanceledException();
        }

        await using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        using StreamReader reader = new StreamReader(fileStream);
        
        var linesBuffer = new List<string>(linesPerChunk);
        int lineCount = 0;

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            linesBuffer.Add(line);
            lineCount++;

            if (lineCount >= linesPerChunk)
            {
                yield return linesBuffer.ToArray();
                linesBuffer.Clear();
                lineCount = 0;
            }
        }

        
        if (linesBuffer.Count > 0)
        {
            yield return linesBuffer.ToArray();
        }
    }
    
    private string? GetFilePathFromUser()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
        
        string? filePath = null;
        
        if (openFileDialog.ShowDialog() == true)
        {
            filePath = openFileDialog.FileName;
            FileValidator.ValidateAndCheckAccess(filePath);
        }

        return filePath;
    }
}