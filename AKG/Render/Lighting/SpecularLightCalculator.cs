using System.Numerics;

namespace AKG.Render.Lighting;


public sealed class SpecularLightCalculator
{
    private readonly float _specularCoefficient;
    private readonly float _shininess;
    private readonly Vector3 _specularColor;
    private readonly Vector3 _lightPosition;

    public SpecularLightCalculator(
        float specularCoefficient, 
        float shininess, 
        Vector3 specularColor,
        Vector3 lightPosition)
    {
        _specularCoefficient = specularCoefficient;
        _shininess = shininess;
        _specularColor = specularColor;
        _lightPosition = lightPosition;
    }

    public Vector3 Calculate(Vector3 worldPosition, Vector3 normal, Vector3 cameraPosition)
    {
        Vector3 lightDirection = Vector3.Normalize(_lightPosition - worldPosition);
        
        Vector3 viewDirection = Vector3.Normalize(cameraPosition - worldPosition);
        
        Vector3 reflectDirection = CalculateReflection(lightDirection, normal);
        
        float specularFactor = Math.Max(Vector3.Dot(reflectDirection, viewDirection), 0f);
        
        specularFactor = (float)Math.Pow(specularFactor, _shininess);
        
        return _specularCoefficient * specularFactor * _specularColor;
    }

    private Vector3 CalculateReflection(Vector3 incident, Vector3 normal)
    {
        return incident - 2 * Vector3.Dot(incident, normal) * normal;
    }
}