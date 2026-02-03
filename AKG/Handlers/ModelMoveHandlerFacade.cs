using System.Windows.Input;

namespace AKG.Handlers;

public class ModelMoveHandlerFacade
{
    private readonly ObjRenderer _renderer;
    private const float MoveStep = 0.2f;
    private const float ZoomStep = 0.5f;
    
    private delegate void KeyHandler();  
    
    private readonly Dictionary<Key, KeyHandler> _moveHandlers;

    public ModelMoveHandlerFacade(ObjRenderer renderer)
    {
        _renderer = renderer;

        _moveHandlers = new Dictionary<Key, KeyHandler>
        {
            [Key.W] = HandleUpMove,
            [Key.S] = HandleDownMove,
            [Key.A] = HandleLeftMove,
            [Key.D] = HandleRightMove,
            [Key.Add] = () => renderer.ZoomIn(ZoomStep),
            [Key.Subtract] = () => renderer.ZoomOut(ZoomStep),
            [Key.OemPlus] = () => renderer.ZoomIn(ZoomStep),
            [Key.OemMinus] = () => renderer.ZoomOut(ZoomStep),
            [Key.R] = () => renderer.ResetView(),
            [Key.Escape] = () => renderer.ResetView()
        };
    }

    public void HandleKeyPress(in Key key)
    {
        if (_moveHandlers.TryGetValue(key, out KeyHandler? handler))
        {
            handler.Invoke();
        }
    }

    public void HandleUpMove()
    {
        _renderer.MoveUp(MoveStep);
    }

    public void HandleDownMove()
    {
        _renderer.MoveDown(MoveStep);
    }

    public void HandleLeftMove()
    {
        _renderer.MoveLeft(MoveStep);
    }

    public void HandleRightMove()
    {
        _renderer.MoveRight(MoveStep);
    }
}