// ================ RenderConstants.cs ================
namespace AKG.Render.Constants;

public static class RenderConstants
{
    public const float InitialZoom = 5.0f;
    public const float MinimumZoom = 0.5f;
    public const float MaximumZoom = 200.0f;
    
    public const float MinimumModelScale = 0.001f;
    public const float MaximumModelScale = 100.0f;
    
    public const float DefaultFieldOfView = MathF.PI / 3;
    
    public const float DefaultNearClippingPlane = 0.1f;
    public const float DefaultFarClippingPlane = 10000f;
    
    public const float DefaultNormalizationFactor = 2.0f;
    
    public const float ParallelProcessingThreshold = 1000;
    
    public const float LineDrawingThreshold = 1000;
    
    public const float DepthDivisionThreshold = 0.0001f;
    
    public const float CoordinateSystemNormalizationFactor = 5.0f;
    
    public const float MouseDragRotationFactor = 0.01f;
    public const float MouseWheelZoomFactor = 0.001f;
    
    public const float InitialHalfCoordinateSystem = 0.5f;
    
    public const uint BlackColor = 0xFF000000;
    public const uint WhiteColor = 0xFFFFFFFF;

    public const int BytesPerPixel = 4;
}