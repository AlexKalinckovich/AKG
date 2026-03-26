using System.Numerics;

namespace AKG.Render.Lighting;


public sealed class DiffuseLightCalculator
{
    private readonly float _diffuseCoefficient;
    private readonly Vector3 _lightColor;
    private readonly Vector3 _lightPosition;

    public Vector3 LightPosition => _lightPosition;    
    public DiffuseLightCalculator(
        float diffuseCoefficient, 
        Vector3 lightColor, 
        Vector3 lightPosition)
    {
        _diffuseCoefficient = diffuseCoefficient;
        _lightColor = lightColor;
        _lightPosition = lightPosition;
    }

    public Vector3 Calculate(Vector3 worldPosition, Vector3 normal)
    {
        Vector3 lightDirection = Vector3.Normalize(_lightPosition - worldPosition);
        
        float diffuseFactor = Math.Max(Vector3.Dot(normal, lightDirection), 0f);
        
        return _diffuseCoefficient * diffuseFactor * _lightColor;
    }

    public Vector3 Calculate_V2(Vector3 lightDirection, Vector3 normal)
    {
        float diffuseFactor = Math.Max(Vector3.Dot(normal, lightDirection), 0f);
        
        return _diffuseCoefficient * diffuseFactor * _lightColor;
    }
}