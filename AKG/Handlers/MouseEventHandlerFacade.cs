
using System.Windows;
using System.Windows.Input;

namespace AKG.Handlers;

public class MouseEventHandlerFacade
{
    private readonly ObjRenderer _renderer;
    private Point? _lastMousePosition;
    private bool _isRotating = false;
        
    public MouseEventHandlerFacade(ObjRenderer renderer)
    {
        _renderer = renderer;
    }
        
    public void HandleMouseDown(Point position)
    {
        _renderer.OnMouseDown(position);
    }
    
    public void HandleMouseMove(Point position)
    {
        if (!_isRotating || _lastMousePosition == null) 
            return;
            
        _renderer.OnMouseMove(position);
        _lastMousePosition = position;
    }
        
    public void HandleMouseUp()
    {
        _isRotating = false;
        _lastMousePosition = null;
    }
        
    public void HandleMouseWheel(int delta)
    {
        _renderer.OnMouseWheel(delta);
    }
}