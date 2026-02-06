using System.Numerics;
using AKG.Render.Constants;

namespace AKG.Render.States;

// ================ CameraState.cs ================

public class CameraState
{
    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public float Zoom { get; set; } = RenderConstants.InitialZoom;
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    public Vector3 EyePosition => new(OffsetX, OffsetY, Zoom);
    
    public Vector3 TargetPosition => new(OffsetX, OffsetY, 0);
    
    public Vector3 UpDirection => new(0, 1, 0);

    public void ResetToDefault()
    {
        RotationX = 0;
        RotationY = 0;
        Zoom = RenderConstants.InitialZoom;
        OffsetX = 0;
        OffsetY = 0;
    }

    public void ModifyZoom(float amount)
    {
        Zoom -= amount;
        Zoom = Math.Clamp(Zoom, RenderConstants.MinimumZoom, RenderConstants.MaximumZoom);
    }

    public void ModifyOffsetX(float amount)
    {
        OffsetX += amount;
    }

    public void ModifyOffsetY(float amount)
    {
        OffsetY += amount;
    }

    public void ModifyRotationX(float amount)
    {
        RotationX += amount;
    }

    public void ModifyRotationY(float amount)
    {
        RotationY += amount;
    }
}