using System.Drawing;
using System.Numerics;
using Bitmap = System.Drawing.Bitmap;
namespace AKG.Render.Texture;

public static class TextureFileReader
{
    public static Texture2D TryLoadTextureFromFile(string filePath)
    {
       try
       {
           return LoadTextureFromFileAction(filePath);
       }
       catch (Exception)
       {
           return HandleTextureLoadException();
       }
    }

    private static Texture2D LoadTextureFromFileAction(string filePath)
    {
        using Bitmap bitmap = new Bitmap(filePath);
            
        int width = bitmap.Width;
            
        int height = bitmap.Height;
            
            
        Vector3[,] pixels = LoadPixelsFromBitmap(bitmap, width, height);
            
        return new Texture2D(width, height, pixels);
    }

    private static Vector3[,] LoadPixelsFromBitmap(Bitmap bitmap, int width, int height)
    {
        Vector3[,] pixels = new Vector3[width, height];

        for (int y = 0; y < height; y++)
        {
            LoadPixelRow(bitmap, pixels, y, width);
        }
            
        return pixels;
    }

    private static void LoadPixelRow(Bitmap bitmap, Vector3[,] pixels, int y, int width)
    {
        for (int x = 0; x < width; x++)
        {
            SetPixelFromBitmap(bitmap, pixels, x, y);
        }
    }

    private static void SetPixelFromBitmap(Bitmap bitmap, Vector3[,] pixels, int x, int y)
    {
        Color color = bitmap.GetPixel(x, y);
        pixels[x, y] = NormalizeColor(color);
    }

    private static Vector3 NormalizeColor(Color color)
    {
        float maxColorValue = 255.0f;
        float r = color.R / maxColorValue;
        float g = color.G / maxColorValue;
        float b = color.B / maxColorValue;
        return new Vector3(r, g, b);
    }

    private static Texture2D HandleTextureLoadException()
    {
        return ProceduralTextureGenerator.CreateFallbackMap();
    }
}