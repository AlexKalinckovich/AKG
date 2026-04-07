using AKG.Render.Texture;

namespace AKG.Model;

public struct RenderTextureMaps
{
    public Texture2D DiffuseMap { get; init; }
    public Texture2D NormalMap { get; init; }
    public Texture2D SpecularMap { get; init; }

    public RenderTextureMaps(Texture2D diffuseMap, Texture2D normalMap, Texture2D specularMap)
    {
        DiffuseMap = diffuseMap;
        NormalMap = normalMap;
        SpecularMap = specularMap;
    }
    
    public static RenderTextureMaps CreateDefaultRenderMaps()
    {
        Texture2D diffuseMap = ProceduralTextureGenerator.CreateDiffuseMap();
        Texture2D normalMap = ProceduralTextureGenerator.CreateNormalMap();
        Texture2D specularMap = ProceduralTextureGenerator.CreateSpecularMap();
        return new RenderTextureMaps(diffuseMap, normalMap, specularMap);
    }
}