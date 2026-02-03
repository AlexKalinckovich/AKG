using System.Buffers;
using System.IO;
using Microsoft.Win32;

namespace AKG.FileHelper;

public class ObjFileLoader
{
    public async Task<FileChunk> LoadEntireFileAsync()
    {
        string? filePath = GetFilePathFromUser();
        if (filePath is null)
        {
            throw new OperationCanceledException();
        }

        long fileSize = new FileInfo(filePath).Length;
       
        byte[] buffer = ArrayPool<byte>.Shared.Rent((int)fileSize);
        
        using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        
        int bytesRead = await fileStream.ReadAsync(buffer, 0, (int)fileSize);
        
        return new FileChunk(buffer, bytesRead);
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