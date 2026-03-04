using System.Numerics;
using AKG.Render.Constants;

namespace AKG.Render.Lighting;


public sealed class LightingCalculator
{
    private readonly AmbientLightCalculator _ambientCalculator;
    private readonly DiffuseLightCalculator _diffuseCalculator;
    private readonly SpecularLightCalculator _specularCalculator;

    public LightingCalculator(
        AmbientLightCalculator ambientCalculator,
        DiffuseLightCalculator diffuseCalculator,
        SpecularLightCalculator specularCalculator)
    {
        _ambientCalculator = ambientCalculator;
        _diffuseCalculator = diffuseCalculator;
        _specularCalculator = specularCalculator;
    }

    public uint CalculatePixelColor(
        Vector3 worldPosition, 
        Vector3 normal, 
        Vector3 cameraPosition)
    {
        normal = Vector3.Normalize(normal);

        Vector3 ambient = _ambientCalculator.Calculate();
        Vector3 diffuse = _diffuseCalculator.Calculate(worldPosition, normal);
        Vector3 specular = _specularCalculator.Calculate(worldPosition, normal, cameraPosition);

        Vector3 finalColor = ambient + diffuse + specular;
        finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);

        return ConvertToUIntColor(finalColor);
    }

    private uint ConvertToUIntColor(Vector3 color)
    {
        byte red = (byte)(color.X * RasterizationConstants.MaxColorValue);
        byte green = (byte)(color.Y * RasterizationConstants.MaxColorValue);
        byte blue = (byte)(color.Z * RasterizationConstants.MaxColorValue);

        return (uint)((RasterizationConstants.MaxColorValue << RasterizationConstants.AlphaChannelShift) |
                      (red << RasterizationConstants.RedChannelShift) |
                      (green << RasterizationConstants.GreenChannelShift) |
                      (blue << RasterizationConstants.BlueChannelShift));
    }
}