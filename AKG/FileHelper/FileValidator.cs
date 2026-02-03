using System.IO;

namespace AKG.FileHelper;

public static class FileValidator
{
    public static void ValidateAndCheckAccess(string? filePath)
    {
        AssertPathNotNullOrEmpty(filePath);

        AssertFileExists(filePath);

        AssertFileCanOpen(filePath);
    }

    private static void AssertFileCanOpen(string? filePath)
    {
        try
        {
            using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            { }
        }
        catch (IOException ex) when (IsFileLocked(ex))
        {
            throw new IOException($"The file '{filePath}' is currently in use by another process.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access to '{filePath}' is denied. Check your permissions.", ex);
        }
    }

    private static void AssertFileExists(string? filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file '{filePath}' was not found.");
    }

    private static void AssertPathNotNullOrEmpty(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");
    }

    private static bool IsFileLocked(IOException exception)
    {
        int errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        return errorCode == 32 || errorCode == 33;
    }
}