namespace AKG.Render.Texture;

using System.Numerics;


public sealed class Texture2D
{
    private readonly Vector3[,] _pixels;
    private readonly int _width;
    private readonly int _height;

    public int Width => _width;
    public int Height => _height;

    public Texture2D(int width, int height)
    {
        _width = width;
        _height = height;
        _pixels = new Vector3[width, height];
    }

    public Texture2D(int width, int height, Vector3[,] pixelData)
    {
        _width = width;
        _height = height;
        _pixels = pixelData;
    }

    public Vector3 GetPixel(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return Vector3.Zero;
        }

        return _pixels[x, y];
    }

    public void SetPixel(int x, int y, Vector3 color)
    {
        bool isPixelInBound = (x >= 0 && x < _width) &&
                              (y >= 0 && y < _height);
        if (isPixelInBound)
        {
            _pixels[x, y] = color;
        }
    }

    public Vector3 Sample(float u, float v)
    {
        int x = (int)(u * _width);
        int y = (int)(v * _height);

        x = Math.Clamp(x, 0, _width - 1);
        y = Math.Clamp(y, 0, _height - 1);

        return _pixels[x, y];
    }

    public Vector3 SampleBilinear(float u, float v)
    {
        float fx = u * _width - 0.5f;
        float fy = v * _height - 0.5f;

        int x0 = (int)Math.Floor(fx);
        int y0 = (int)Math.Floor(fy);
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        float dx = fx - x0;
        float dy = fy - y0;

        x0 = Math.Clamp(x0, 0, _width - 1);
        x1 = Math.Clamp(x1, 0, _width - 1);
        y0 = Math.Clamp(y0, 0, _height - 1);
        y1 = Math.Clamp(y1, 0, _height - 1);

        Vector3 c00 = GetPixel(x0, y0);
        Vector3 c01 = GetPixel(x0, y1);
        Vector3 c10 = GetPixel(x1, y0);
        Vector3 c11 = GetPixel(x1, y1);

        Vector3 c0 = Vector3.Lerp(c00, c10, dx);
        Vector3 c1 = Vector3.Lerp(c01, c11, dx);

        return Vector3.Lerp(c0, c1, dy);
    }

    public static Texture2D CreateFromColors(int width, int height, uint[] colors)
    {
        Vector3[,] pixels = new Vector3[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (index < colors.Length)
                {
                    uint color = colors[index];

                    byte r = (byte)((color >> 16) & 0xFF);
                    byte g = (byte)((color >> 8) & 0xFF);
                    byte b = (byte)(color & 0xFF);

                    const float maxColorValue = 255.0f;
                    pixels[x, y] = new Vector3(
                        r / maxColorValue,
                        g / maxColorValue,
                        b / maxColorValue
                    );
                }
            }
        }

        return new Texture2D(width, height, pixels);
    }

    public static Texture2D CreateSolidColor(int width, int height, Vector3 color)
    {
        Vector3[,] pixels = new Vector3[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[x, y] = color;
            }
        }

        return new Texture2D(width, height, pixels);
    }

    public static Texture2D CreateNormalMapDefault(int width, int height)
    {
        Vector3 defaultNormal = new Vector3(0.0f, 0.0f, 1.0f);
        return CreateSolidColor(width, height, defaultNormal);
    }

    public static Texture2D CreateSpecularMapDefault(int width, int height, float intensity)
    {
        Vector3 color = new Vector3(intensity, intensity, intensity);
        return CreateSolidColor(width, height, color);
    }
}