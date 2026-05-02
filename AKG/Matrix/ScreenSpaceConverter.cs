using System.Numerics;
using System.Windows;
using AKG.Render.Constants;

namespace AKG.Matrix;

public sealed class ScreenSpaceConverter
{
    private readonly float _halfWidth;
    private readonly float _halfHeight;

    public ScreenSpaceConverter(int viewportWidth, int viewportHeight)
    {
        _halfWidth = viewportWidth * RenderConstants.InitialHalfCoordinateSystem; 
        _halfHeight = viewportHeight * RenderConstants.InitialHalfCoordinateSystem;
    }

    public Point ConvertToScreenSpace(Vector4 clipPosition)
    {
        
        if (Math.Abs(clipPosition.W) < RenderConstants.DepthDivisionThreshold)
        {
            return new Point(-10000, -10000); 
        }

        float normalizedX = clipPosition.X / clipPosition.W;
        float normalizedY = clipPosition.Y / clipPosition.W;
        
        double screenX = (normalizedX + 1) * _halfWidth;
        
        double screenY = (1 - normalizedY) * _halfHeight;
        
        return new Point(screenX, screenY);
    }
}