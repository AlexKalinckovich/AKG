using System.Windows;
using AKG.Matrix;
using AKG.Render;
using AKG.Render.Constants;
using AKG.Render.States;

namespace AKG.Handlers;


public class MouseInteractionHandler
{
    private readonly CameraState _cameraState;
    private readonly TransformationMatrixManager _matrixManager;
    private bool _isDragging = false;
    private Point _previousMousePosition;

    public MouseInteractionHandler(CameraState cameraState, TransformationMatrixManager matrixManager)
    {
        _cameraState = cameraState;
        _matrixManager = matrixManager;
    }

    public void HandleMouseDown(Point position)
    {
        _isDragging = true;
        _previousMousePosition = position;
    }

    public void HandleMouseMove(Point position)
    {
        if (!_isDragging)
        {
            return;
        }

        UpdateCameraRotation(position);
        UpdatePreviousMousePosition(position);
        MarkMatricesAsDirty();
    }

    public void HandleMouseUp()
    {
        _isDragging = false;
    }

    public void HandleMouseWheel(int delta)
    {
        float zoomChange = delta * RenderConstants.MouseWheelZoomFactor * _cameraState.Zoom;
        _cameraState.ModifyZoom(zoomChange);
        MarkMatricesAsDirty();
    }

    private void UpdateCameraRotation(Point currentPosition)
    {
        float deltaX = (float)(currentPosition.X - _previousMousePosition.X);
        float deltaY = (float)(currentPosition.Y - _previousMousePosition.Y);

        _cameraState.ModifyRotationY(deltaX * RenderConstants.MouseDragRotationFactor);
        _cameraState.ModifyRotationX(deltaY * RenderConstants.MouseDragRotationFactor);
    }

    private void UpdatePreviousMousePosition(Point currentPosition)
    {
        _previousMousePosition = currentPosition;
    }

    private void MarkMatricesAsDirty()
    {
        _matrixManager.MarkMatricesAsDirty();
    }
}