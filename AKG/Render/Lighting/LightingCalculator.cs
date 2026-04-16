using System.Numerics;
using AKG.Model;
using AKG.Render.Constants;
using AKG.Render.Texture;

namespace AKG.Render.Lighting;

public sealed class LightingCalculator
{
    private readonly AmbientLightCalculator _ambientCalculator;
    private readonly DiffuseLightCalculator _diffuseCalculator;
    private readonly SpecularLightCalculator _specularCalculator;
    public RenderTextureMaps TextureMaps { get; set; }
    public LightingCalculator(
        AmbientLightCalculator ambientCalculator,
        DiffuseLightCalculator diffuseCalculator,
        SpecularLightCalculator specularCalculator)
    {
        _ambientCalculator = ambientCalculator;
        _diffuseCalculator = diffuseCalculator;
        _specularCalculator = specularCalculator;

        TextureMaps = ProceduralTextureGenerator.CreateDefaultTextureMap();
    }


    public uint CalculatePixelColor(
        Vector3 interpolatedWorldPosition,
        Vector3 cameraPosition,
        Vector2 uvCoordinates)
    {
        Vector3 normal = GetNormal(uvCoordinates, TextureMaps.NormalMap);

        Vector3 lightDirection = Vector3.Normalize(_diffuseCalculator.LightPosition - interpolatedWorldPosition);
       
        Vector3 viewDirection = Vector3.Normalize(cameraPosition - interpolatedWorldPosition);

        Vector3 diffuseColor = TextureMaps.DiffuseMap.Sample(uvCoordinates.X, uvCoordinates.Y);

        Vector3 ambient = _ambientCalculator.Calculate(diffuseColor);

        Vector3 diffuse = _diffuseCalculator.CalculateDiffuseColor(lightDirection, normal, diffuseColor);

        Vector3 specularColor = TextureMaps.SpecularMap.Sample(uvCoordinates.X, uvCoordinates.Y);
            
        float specularIntensity = specularColor.X;

        Vector3 specular = _specularCalculator.CalculateSpecular(lightDirection, viewDirection, normal, specularIntensity);

        return CalculateFinalColor(ambient, diffuse, specular);
    }

    private uint CalculateFinalColor(Vector3 ambient, Vector3 diffuse, Vector3 specular)
    {
        Vector3 finalColor = ambient + diffuse + specular;
        
        finalColor = Vector3.Clamp(finalColor, min: Vector3.Zero, max: Vector3.One);

        return ConvertToUIntColor(finalColor);
    }


    private static Vector3 GetNormal(Vector2 uvCoordinates, Texture2D normalMap)
    {
        Vector3 normalFromTexture = normalMap.Sample(u: uvCoordinates.X, v: uvCoordinates.Y);
            
        Vector3 normal = normalFromTexture * 2.0f - Vector3.One;
        
        return Vector3.Normalize(normal);
    }

    private static uint ConvertToUIntColor(Vector3 color)
    {
        byte red   = (byte)(color.X * RasterizationConstants.MaxColorValue);
        byte green = (byte)(color.Y * RasterizationConstants.MaxColorValue);
        byte blue  = (byte)(color.Z * RasterizationConstants.MaxColorValue);

        return (uint)((RasterizationConstants.MaxColorValue << RasterizationConstants.AlphaChannelShift) |
                      (red << RasterizationConstants.RedChannelShift) |
                      (green << RasterizationConstants.GreenChannelShift) |
                      (blue << RasterizationConstants.BlueChannelShift));
    }
}