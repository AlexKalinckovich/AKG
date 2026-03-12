using System.Numerics;

namespace AKG.Render.Lighting;


public sealed class AmbientLightCalculator
{
    private readonly float _ambientCoefficient;
    private readonly Vector3 _ambientLightColor;

    public AmbientLightCalculator(float ambientCoefficient, Vector3 ambientLightColor)
    {
        _ambientCoefficient = ambientCoefficient;
        
        _ambientLightColor = ambientLightColor;
    }

    public Vector3 Calculate()
    {
        return _ambientCoefficient * _ambientLightColor;
    }
}