using System.Numerics;
using AKG.Model;

namespace AKG.Render.Texture;

public static class ProceduralTextureGenerator
{
    private const int DefaultSize = 256;

    public static RenderTextureMaps CreateDefaultTextureMap()
    {
        return new RenderTextureMaps(
            CreateDiffuseMap(),
            CreateNormalMap(),
            CreateSpecularMap()
        );
    }
    public static Texture2D CreateDiffuseMap()
    {
        Vector3 color1 = new Vector3(0.8f, 0.2f, 0.2f);
        Vector3 color2 = new Vector3(0.2f, 0.8f, 0.2f);
        return CreateCheckerboard(DefaultSize, DefaultSize, 32, color1, color2);
    }

    public static Texture2D CreateNormalMap()
    {
        return CreateNormalMapBumped(DefaultSize, DefaultSize, 0.5f);
    }

    public static Texture2D CreateSpecularMap()
    {
        float intensity = 0.5f;
        Vector3 color = new Vector3(intensity, intensity, intensity);
        return CreateSolidColor(DefaultSize, DefaultSize, color);
    }

    public static Texture2D CreateFallbackMap()
    {
        return CreateSolidColor(DefaultSize, DefaultSize, Vector3.One);
    }

    private static Texture2D CreateCheckerboard(int width, int height, int tileSize, Vector3 color1, Vector3 color2)
    {
        Vector3[,] pixels = new Vector3[width, height];
        ProcessCheckerboardRows(pixels, width, height, tileSize, color1, color2);
        return new Texture2D(width, height, pixels);
    }

    private static void ProcessCheckerboardRows(Vector3[,] pixels, int width, int height, int tileSize, Vector3 color1, Vector3 color2)
    {
        for (int y = 0; y < height; y++)
        {
            ProcessCheckerboardRow(pixels, width, y, tileSize, color1, color2);
        }
    }

    private static void ProcessCheckerboardRow(Vector3[,] pixels, int width, int y, int tileSize, Vector3 color1, Vector3 color2)
    {
        for (int x = 0; x < width; x++)
        {
            AssignCheckerboardPixel(pixels, x, y, tileSize, color1, color2);
        }
    }

    private static void AssignCheckerboardPixel(Vector3[,] pixels, int x, int y, int tileSize, Vector3 color1, Vector3 color2)
    {
        int tileX = x / tileSize;
        int tileY = y / tileSize;
        bool isEven = (tileX + tileY) % 2 == 0;
        pixels[x, y] = isEven ? color1 : color2;
    }

    private static Texture2D CreateNormalMapBumped(int width, int height, float bumpIntensity)
    {
        float centerX = width / 2.0f;
        float centerY = height / 2.0f;
        Vector3[,] pixels = ProcessBumpedNormalRows(width, height, bumpIntensity, centerX, centerY);
        return new Texture2D(width, height, pixels);
    }

    private static Vector3[,] ProcessBumpedNormalRows(int width, int height, float bumpIntensity, float centerX, float centerY)
    {
        Vector3[,] pixels = new Vector3[width, height];
        for (int y = 0; y < height; y++)
        {
            ProcessBumpedNormalRow(pixels, width,height, y, bumpIntensity, centerX, centerY);
        }
        return pixels;
    }

    private static void ProcessBumpedNormalRow(Vector3[,] pixels, int width, int height, int y, float bumpIntensity, float centerX, float centerY)
    {
        for (int x = 0; x < width; x++)
        {
            ComputeBumpedNormal(pixels, x, y, width, height, bumpIntensity, centerX, centerY);
        }
    }

    private static void ComputeBumpedNormal(Vector3[,] pixels, int x, int y, int width, int height, float bumpIntensity, float centerX, float centerY)
    {
        float dx = (x - centerX) / width;
        float dy = (y - centerY) / height;
        float nx = -dx * bumpIntensity;
        float ny = -dy * bumpIntensity;
        float nz = ComputeNormalZ(nx, ny);
        pixels[x, y] = PackNormal(nx, ny, nz);
    }

    private static float ComputeNormalZ(float nx, float ny)
    {
        return 1.0f - (float)Math.Sqrt(nx * nx + ny * ny);
    }

    private static Vector3 PackNormal(float nx, float ny, float nz)
    {
        float packedNx = nx * 0.5f + 0.5f;
        float packedNy = ny * 0.5f + 0.5f;
        float packedNz = nz * 0.5f + 0.5f;
        return new Vector3(packedNx, packedNy, packedNz);
    }

    private static Texture2D CreateSolidColor(int width, int height, Vector3 color)
    {
        Vector3[,] pixels = new Vector3[width, height];
        FillSolidColorRows(pixels, width, height, color);
        return new Texture2D(width, height, pixels);
    }

    private static void FillSolidColorRows(Vector3[,] pixels, int width, int height, Vector3 color)
    {
        for (int y = 0; y < height; y++)
        {
            FillSolidColorRow(pixels, width, y, color);
        }
    }

    private static void FillSolidColorRow(Vector3[,] pixels, int width, int y, Vector3 color)
    {
        for (int x = 0; x < width; x++)
        {
            pixels[x, y] = color;
        }
    }
}