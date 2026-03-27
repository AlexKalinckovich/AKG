using System.Numerics;

namespace AKG.Render.Texture;

public class TextureLoader
{
    public static Texture2D CreateProceduralDiffuseMap()
    {
        return Texture2D.CreateCheckerboard(256, 256, 32,
            new Vector3(0.8f, 0.2f, 0.2f),
            new Vector3(0.2f, 0.8f, 0.2f));
    }

    public static Texture2D CreateProceduralNormalMap()
    {
        return Texture2D.CreateNormalMapBumped(256, 256, 0.5f);
    }

    public static Texture2D CreateProceduralSpecularMap()
    {
        return Texture2D.CreateSpecularMapUniform(256, 256, 0.5f);
    }
}