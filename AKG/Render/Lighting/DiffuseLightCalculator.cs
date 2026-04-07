using System.Numerics;

namespace AKG.Render.Lighting;

public sealed class DiffuseLightCalculator
{
    private readonly float _diffuseCoefficient;
    private readonly Vector3 _lightColor;

    public Vector3 LightPosition { get; }

    public DiffuseLightCalculator(
        float diffuseCoefficient,
        Vector3 lightColor,
        Vector3 lightPosition)
    {
        _diffuseCoefficient = diffuseCoefficient;
        _lightColor = lightColor;
        LightPosition = lightPosition;
    }

    public Vector3 CalculateDiffuseColor(Vector3 lightDirection, Vector3 normal, Vector3 diffuseColor)
    {
        float diffuseFactor = Math.Max(Vector3.Dot(normal, lightDirection), 0f);

        return _diffuseCoefficient * diffuseFactor * _lightColor * diffuseColor;
    }
}