using AKG.Render.Constants;

namespace AKG.Render.Lighting;

public static class LightCalculatorBuilder
{
    public static LightingCalculator CreateLightingCalculator()
    {
        return new LightingCalculator(
            CreateAmbientCalculator(),
            CreateDiffuseCalculator(),
            CreateSpecularCalculator()
        );
    }

    private static AmbientLightCalculator CreateAmbientCalculator()
    {
        return new AmbientLightCalculator(
            LightingConstants.AmbientCoefficient,
            LightingConstants.DefaultAmbientLightColor);
    }

    private static DiffuseLightCalculator CreateDiffuseCalculator()
    {
        return new DiffuseLightCalculator(
            LightingConstants.DiffuseCoefficient,
            LightingConstants.DefaultLightColor,
            LightingConstants.DefaultLightPosition);
    }

    private static SpecularLightCalculator CreateSpecularCalculator()
    {
        return new SpecularLightCalculator(
            LightingConstants.SpecularCoefficient,
            LightingConstants.Shininess,
            LightingConstants.DefaultSpecularColor,
            LightingConstants.DefaultLightPosition);
    }
}