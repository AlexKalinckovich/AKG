using System.Numerics;


namespace AKG.Render.Constants;

public static class LightingConstants
{
    public const float AmbientCoefficient = 0.2f;
    public const float DiffuseCoefficient = 0.8f;
    public const float SpecularCoefficient = 0.5f;
    public const float MinimumIntensity = 0.2f;
    public const float MaximumIntensity = 1.0f;
    public const float Shininess = 32f;
    public const float Epsilon = 1e-10f;
    
    public static readonly Vector3 DefaultLightDirection = new Vector3(0, 0, 1);
    public static readonly Vector3 DefaultLightPosition = new Vector3(0, 5, 10);
    public static readonly Vector3 DefaultLightColor = new Vector3(1, 1, 1);
    public static readonly Vector3 DefaultAmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
    public static readonly Vector3 DefaultSpecularColor = new Vector3(1, 1, 1);
}