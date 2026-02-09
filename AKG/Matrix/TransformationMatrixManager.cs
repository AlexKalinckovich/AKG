// ================ TransformationMatrixManager.cs ================

using AKG.Model;
using AKG.Render.Constants;
using AKG.Render.States;

namespace AKG.Matrix;

public class TransformationMatrixManager
{
    private readonly CameraState _cameraState;
    private readonly ModelState _modelState;
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;

    private Matrix4 _modelMatrix;
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;
    private bool _areMatricesDirty = true;

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
        Matrix4 translationToCenter = Matrix4.Translate(-_modelState.Center.X, -_modelState.Center.Y, -_modelState.Center.Z);
        
        Matrix4 scale = Matrix4.Scale(_modelState.Scale, _modelState.Scale, _modelState.Scale);
        
        Matrix4 rotation = Matrix4.RotateY(_cameraState.RotationY) * Matrix4.RotateX(_cameraState.RotationX);
        
        _modelMatrix = rotation * scale * translationToCenter;
    }

    private void UpdateViewMatrix()
    {
        _viewMatrix = Matrix4.LookAt(
            _cameraState.EyePosition,
            _cameraState.TargetPosition,
            _cameraState.UpDirection
        );
    }

    private void UpdateProjectionMatrix()
    {
        float aspectRatio = (float)_viewportWidth / _viewportHeight;
        
        _projectionMatrix = Matrix4.Perspective(
            RenderConstants.DefaultFieldOfView,
            aspectRatio,
            RenderConstants.DefaultZNearClippingPlane,
            RenderConstants.DefaultZFarClippingPlane
        );
    }

    public Matrix4 ModelMatrix => _modelMatrix;
    public Matrix4 ViewMatrix => _viewMatrix;
    public Matrix4 ProjectionMatrix => _projectionMatrix;
}