using System.Numerics;
using AKG.Render.Constants;
using AKG.Render.States;

namespace AKG.Matrix;

public class TransformationMatrixManager
{
    private readonly CameraState _cameraState;
    private readonly ModelState _modelState;
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;

    private bool _areMatricesDirty = true;

    public Matrix4x4 ModelMatrix { get; private set; }
    public Matrix4x4 ViewMatrix { get; private set; }
    public Matrix4x4 ProjectionMatrix { get; private set; }

    public TransformationMatrixManager(CameraState cameraState,
                                       ModelState modelState,
                                       int viewportWidth,
                                       int viewportHeight)
    {
        _cameraState = cameraState;
        _modelState = modelState;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
    }

    public void MarkMatricesAsDirty()
    {
        _areMatricesDirty = true;
    }

    public void UpdateMatricesIfNeeded()
    {
        if (_areMatricesDirty)
        {
            UpdateModelMatrix();
            UpdateViewMatrix();
            UpdateProjectionMatrix();
            _areMatricesDirty = false;
        }
    }

    private void UpdateModelMatrix()
    {
        Matrix4x4 translationToCenter = Matrix4x4.CreateTranslation(-_modelState.Center.X, -_modelState.Center.Y, -_modelState.Center.Z);
        Matrix4x4 scale = Matrix4x4.CreateScale(_modelState.Scale);
        Matrix4x4 rotation = Matrix4x4.CreateRotationY(_cameraState.RotationY) * Matrix4x4.CreateRotationX(_cameraState.RotationX);
        
        ModelMatrix = rotation * scale * translationToCenter;
    }

    private void UpdateViewMatrix()
    {
        ViewMatrix = Matrix4x4.CreateLookAt(
            _cameraState.EyePosition, 
            _cameraState.TargetPosition, 
            CameraState.UpDirection
        );
    }

    private void UpdateProjectionMatrix()
    {
        float aspectRatio = (float)_viewportWidth / _viewportHeight;
        float fov = RenderConstants.DefaultFieldOfView;
        float near = RenderConstants.DefaultZNearClippingPlane;
        float far = RenderConstants.DefaultZFarClippingPlane;
        
        
        float f = 1.0f / (float)Math.Tan(fov * 0.5f);
        float rangeInv = 1.0f / (near - far);
        
        
        ProjectionMatrix = new Matrix4x4(
            f / aspectRatio, 0, 0, 0,                    
            0, f, 0, 0,                                  
            0, 0, (near + far) * rangeInv, -1,          
            0, 0, near * far * 2 * rangeInv, 0           
        );
    }
}