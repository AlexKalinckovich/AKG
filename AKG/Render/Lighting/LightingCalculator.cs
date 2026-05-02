using System.Numerics;
using AKG.Model;
using AKG.Render.Constants;
using AKG.Render.Rasterization;
using AKG.Render.Texture;

namespace AKG.Render.Lighting;

public sealed class LightingCalculator
{
    private readonly AmbientLightCalculator _ambientCalculator;
    private readonly DiffuseLightCalculator _diffuseCalculator;
    private readonly SpecularLightCalculator _specularCalculator;
    public RenderTextureMaps TextureMaps { get; set; }
    public static bool UseDiffuseMap { get; set; } = true;
    public static bool UseNormalMap { get; set; } = true;
    public static bool UseSpecularMap { get; set; } = true;
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
        Vector3 interpolatedNormal,
        Matrix4x4 modelMatrix,
        Vector3 cameraPosition,
        Vector2 uvCoordinates)
    {
        Vector3 normal = UseNormalMap 
            ? GetNormal(uvCoordinates, TextureMaps.NormalMap, modelMatrix) 
            : Vector3.Normalize(interpolatedNormal);
    
        Vector3 lightDirection = Vector3.Normalize(_diffuseCalculator.LightPosition - interpolatedWorldPosition);
        Vector3 viewDirection = Vector3.Normalize(cameraPosition - interpolatedWorldPosition);
    
        
        Vector3 diffuseColor = UseDiffuseMap 
            ? TextureMaps.DiffuseMap.SampleBilinear(uvCoordinates.X, uvCoordinates.Y)
            : new Vector3(0.7f, 0.7f, 0.7f); 

        Vector3 ambient = _ambientCalculator.Calculate(diffuseColor);
        Vector3 diffuse = _diffuseCalculator.CalculateDiffuseColor(lightDirection, normal, diffuseColor);

        
        float specularIntensity = 0.5f; 
        if (UseSpecularMap)
        {
            Vector3 specularColor = TextureMaps.SpecularMap.SampleBilinear(uvCoordinates.X, uvCoordinates.Y);
            specularIntensity = specularColor.X;
        }

        Vector3 specular = _specularCalculator.CalculateSpecular(lightDirection, viewDirection, normal, specularIntensity);

        return CalculateFinalColor(ambient, diffuse, specular);
    }

    private uint CalculateFinalColor(Vector3 ambient, Vector3 diffuse, Vector3 specular)
    {
        Vector3 finalColor = ambient + diffuse + specular;
        
        finalColor = Vector3.Clamp(finalColor, min: Vector3.Zero, max: Vector3.One);

        return ConvertToUIntColor(finalColor);
    }


    private static Vector3 GetNormal(Vector2 uvCoordinates, Texture2D normalMap, Matrix4x4 modelMatrix)
    {
        Vector3 normalFromTexture = normalMap.SampleBilinear(u: uvCoordinates.X, v: uvCoordinates.Y);
        Vector3 localNormal = normalFromTexture * 2.0f - Vector3.One;
    
        
        Vector3 worldNormal = Vector3.TransformNormal(localNormal, modelMatrix);
        
        return Vector3.Normalize(worldNormal);
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