namespace AKG.Render.Constants;


public static class RasterizationConstants
{
    public const int AlphaChannelShift = 24;
    public const int RedChannelShift = 16;
    public const int GreenChannelShift = 8;
    public const int BlueChannelShift = 0;
    public const int MaxColorValue = 255;
    public const float BarycentricEpsilon = 1e-10f;
}