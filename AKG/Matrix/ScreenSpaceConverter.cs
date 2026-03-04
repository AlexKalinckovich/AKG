using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Matrix;

public sealed class ScreenSpaceConverter
{
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;
   
    private readonly float _halfWidth;
    private readonly float _halfHeight;

    public ScreenSpaceConverter(int viewportWidth, int viewportHeight)
    {
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        _halfWidth = viewportWidth * RenderConstants.InitialHalfCoordinateSystem;
        _halfHeight = viewportHeight * RenderConstants.InitialHalfCoordinateSystem;
    }

    public Point ConvertToScreenSpace(Vector4 clipPosition)
    {
        if (!IsValidForScreenConversion(clipPosition))
        {
            return new Point(-1,-1);
        }

        Point screenPoint = CalculateScreenPoint(clipPosition);

        return screenPoint;
    }

    private bool IsValidForScreenConversion(Vector4 clipPosition)
    {
        if (IsDepthInvalid(clipPosition))
        {
            return false;
        }

        float normalizedX = clipPosition.X / clipPosition.W;
        float normalizedY = clipPosition.Y / clipPosition.W;

        return !IsCoordinateOutsideViewableRange(normalizedX, normalizedY);
    }

    private bool IsDepthInvalid(Vector4 clipPosition)
    {
        return Math.Abs(clipPosition.W) < RenderConstants.DepthDivisionThreshold;
    }

    private bool IsCoordinateOutsideViewableRange(float x, float y)
    {
        float threshold = RenderConstants.CoordinateSystemNormalizationFactor;
        return x < -threshold || x > threshold || y < -threshold || y > threshold;
    }

    private Point CalculateScreenPoint(Vector4 clipPosition)
    {
        float normalizedX = clipPosition.X / clipPosition.W;
        float normalizedY = clipPosition.Y / clipPosition.W;

        int screenX = ConvertNormalizedToScreenX(normalizedX);
        int screenY = ConvertNormalizedToScreenY(normalizedY);

        screenX = Math.Clamp(screenX, 0, _viewportWidth - 1);
        screenY = Math.Clamp(screenY, 0, _viewportHeight - 1);

        return new Point(screenX, screenY);
    }

    private int ConvertNormalizedToScreenX(float normalizedCoordinate)
    {
        return (int)((normalizedCoordinate + 1) * _halfWidth);
    }

    private int ConvertNormalizedToScreenY(float normalizedCoordinate)
    {
        return (int)((1 - normalizedCoordinate) * _halfHeight);
    }
}