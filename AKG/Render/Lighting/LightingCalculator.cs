using System.Numerics;
using AKG.Render.Constants;
using AKG.Render.Textures;

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
        return CalculatePixelColor(
            interpolatedWorldPosition,
            interpolatedNormal,
            cameraPosition,
            Vector2.Zero,
            null,
            null,
            null
        );
    }

    public uint CalculatePixelColor(
        Vector3 interpolatedWorldPosition,
        Vector3 interpolatedNormal,
        Vector3 cameraPosition,
        Vector2 uvCoordinates,
        Texture2D? diffuseMap,
        Texture2D? normalMap,
        Texture2D? specularMap)
    {
        Vector3 normal = interpolatedNormal;

        if (normalMap != null)
        {
            Vector3 normalFromTexture = normalMap.Sample(uvCoordinates.X, uvCoordinates.Y);
            normal = normalFromTexture * 2.0f - Vector3.One;
            normal = Vector3.Normalize(normal);
        }
        else
        {
            normal = Vector3.Normalize(interpolatedNormal);
        }

        Vector3 lightDirection = Vector3.Normalize(
            _diffuseCalculator.LightPosition - interpolatedWorldPosition);
        Vector3 viewDirection = Vector3.Normalize(
            cameraPosition - interpolatedWorldPosition);

        Vector3 diffuseColor = diffuseMap != null
            ? diffuseMap.Sample(uvCoordinates.X, uvCoordinates.Y)
            : Vector3.One;

        Vector3 ambient = _ambientCalculator.Calculate(diffuseColor);

        Vector3 diffuse = _diffuseCalculator.Calculate_V2(
            lightDirection, normal, diffuseColor);

        float specularIntensity = specularMap != null
            ? specularMap.Sample(uvCoordinates.X, uvCoordinates.Y).X
            : LightingConstants.SpecularCoefficient;

        Vector3 specular = _specularCalculator.Calculate_V2(
            lightDirection, viewDirection, normal, specularIntensity);

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