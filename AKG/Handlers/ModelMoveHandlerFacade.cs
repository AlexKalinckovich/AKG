using System.Windows.Input;
using AKG.Core.Model;

namespace AKG.Handlers;

public class ModelMoveHandlerFacade
{
    private ObjRenderer _renderer;
    private const float MoveStep = 0.2f;
    private const float ZoomStep = 0.5f;
        
    private delegate void KeyHandler();  
        
    private readonly Dictionary<Key, KeyHandler> _moveHandlers;
    
    public ModelMoveHandlerFacade(ObjRenderer renderer)
    {
        _renderer = renderer;
        _moveHandlers = InitializeHandlers();
    }
        
    public ObjRenderer Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
    
    private Dictionary<Key, KeyHandler> InitializeHandlers()
    {
        return new Dictionary<Key, KeyHandler>
        {
            [Key.W] = HandleUpMove,
            [Key.S] = HandleDownMove,
            [Key.A] = HandleLeftMove,
            [Key.D] = HandleRightMove,
            [Key.Add] = () => _renderer.ZoomIn(ZoomStep),
            [Key.Subtract] = () => _renderer?.ZoomOut(ZoomStep),
            [Key.OemPlus] = () => _renderer?.ZoomIn(ZoomStep),
            [Key.OemMinus] = () => _renderer?.ZoomOut(ZoomStep),
            [Key.R] = () => _renderer?.ResetView(),
            [Key.Escape] = () => _renderer?.ResetView()
        };
    }
    
    public void HandleKeyPress(in Key key)
    {
        if (_renderer == null) return;
            
        if (_moveHandlers.TryGetValue(key, out KeyHandler? handler))
        {
            handler.Invoke();
        }
    }
    
    public void HandleUpMove()
    {
        _renderer?.MoveUp(MoveStep);
    }
    
    public void HandleDownMove()
    {
        _renderer?.MoveDown(MoveStep);
    }
    
    public void HandleLeftMove()
    {
        _renderer?.MoveLeft(MoveStep);
    }
    
    public void HandleRightMove()
    {
        _renderer?.MoveRight(MoveStep);
    }
}