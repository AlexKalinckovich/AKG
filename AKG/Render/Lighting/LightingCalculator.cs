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
        Vector3 interpolatedWorldPosition, 
        Vector3 interpolatedNormal, 
        Vector3 cameraPosition)
    {
        interpolatedNormal = Vector3.Normalize(interpolatedNormal);

        Vector3 ambient = _ambientCalculator.Calculate();
        
        Vector3 diffuse = _diffuseCalculator.Calculate(interpolatedWorldPosition, interpolatedNormal);
        
        Vector3 specular = _specularCalculator.Calculate(interpolatedWorldPosition, interpolatedNormal, cameraPosition);

        Vector3 finalColor = ambient + diffuse + specular;
        
        finalColor = Vector3.Clamp(finalColor, min: Vector3.Zero, max: Vector3.One);

        return ConvertToUIntColor(finalColor);
    }

    private uint ConvertToUIntColor(Vector3 color)
    {
        byte red   = (byte)(color.X * RasterizationConstants.MaxColorValue);
        byte green = (byte)(color.Y * RasterizationConstants.MaxColorValue);
        byte blue  = (byte)(color.Z * RasterizationConstants.MaxColorValue);

        return (uint)((RasterizationConstants.MaxColorValue << RasterizationConstants.AlphaChannelShift) |
                      (red   << RasterizationConstants.RedChannelShift) |
                      (green << RasterizationConstants.GreenChannelShift) |
                      (blue  << RasterizationConstants.BlueChannelShift));
    }
}