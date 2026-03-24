
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

    private Matrix4x4 _modelMatrix;
    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;
    private bool _areMatricesDirty = true;

    public Matrix4x4 ModelMatrix => _modelMatrix;
    public Matrix4x4 ViewMatrix => _viewMatrix;
    public Matrix4x4 ProjectionMatrix => _projectionMatrix;
    
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
        
        _modelMatrix = rotation * scale * translationToCenter;
    }

    private void UpdateViewMatrix()
    {
        _viewMatrix = Matrix4x4.CreateLookAt(
            _cameraState.EyePosition, 
            _cameraState.TargetPosition, 
            _cameraState.UpDirection
        );
    }

    private void UpdateProjectionMatrix()
    {
        float aspectRatio = (float)_viewportWidth / _viewportHeight;
        _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
            RenderConstants.DefaultFieldOfView,
            aspectRatio,
            RenderConstants.DefaultZNearClippingPlane,
            RenderConstants.DefaultZFarClippingPlane
        );
    }
}