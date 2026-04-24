using System.IO;
using AKG.Model;

namespace AKG.Render.Texture;

public static class TextureMapLoader
{
    private const string DefaultDiffusePath  = @"C:\Users\brota\RiderProjects\AKG\AKG\Textures\WoodFloor071_1K-PNG_Color.png";
    private const string DefaultNormalPath   = @"C:\Users\brota\RiderProjects\AKG\AKG\Textures\WoodFloor071_1K-PNG_NormalGL.png";
    private const string DefaultSpecularPath = @"C:\Users\brota\RiderProjects\AKG\AKG\Textures\WoodFloor071_1K-PNG_Roughness.png";

    public static RenderTextureMaps LoadDefaultMaps()
    {
        return LoadOrDefault(DefaultDiffusePath, DefaultNormalPath, DefaultSpecularPath);
    }

    private static RenderTextureMaps LoadOrDefault(string diffusePath, string normalPath, string specularPath)
    {
        bool hasAllFiles = File.Exists(diffusePath) && File.Exists(normalPath) && File.Exists(specularPath);
        if (hasAllFiles)
        {
            return LoadAllMaps(diffusePath, normalPath, specularPath);
        }
        return CreateProceduralMaps();
    }

    private static RenderTextureMaps LoadAllMaps(string diffusePath, string normalPath, string specularPath)
    {
        Texture2D diffuseMap = LoadSingleMap(diffusePath);
        Texture2D normalMap = LoadSingleMap(normalPath);
        Texture2D specularMap = LoadSingleMap(specularPath);
        return new RenderTextureMaps(diffuseMap, normalMap, specularMap);
    }

    private static Texture2D LoadSingleMap(string filePath)
    {
        bool fileExists = File.Exists(filePath);
        if (fileExists)
        {
            return TextureFileReader.TryLoadTextureFromFile(filePath);
        }
        return ProceduralTextureGenerator.CreateFallbackMap();
    }

    private static RenderTextureMaps CreateProceduralMaps()
    {
        Texture2D diffuseMap  = ProceduralTextureGenerator.CreateDiffuseMap();
        Texture2D normalMap   = ProceduralTextureGenerator.CreateNormalMap();
        Texture2D specularMap = ProceduralTextureGenerator.CreateSpecularMap();
        return new RenderTextureMaps(diffuseMap, normalMap, specularMap);
    }
}