
using System.Windows;
using System.Windows.Media.Imaging;
using AKG.Core.Model;
using AKG.Handlers;
using AKG.Matrix;
using AKG.Model;
using AKG.Model.Vertex;
using AKG.Render.Renderers;
using AKG.Render.States;

namespace AKG.Render;

public sealed class ObjRenderer
{
   public ObjModel Model { get; set; } = new();

    private readonly CameraState _cameraState;
    private readonly ModelState _modelState;
    private readonly TransformationMatrixManager _matrixManager;
    private readonly VertexTransformer _vertexTransformer;
    private readonly BitmapRenderer _bitmapRenderer;
    private readonly FaceRenderer _faceRenderer;
    private readonly MouseInteractionHandler _mouseHandler;

    public ObjRenderer(WriteableBitmap bitmap)
    {
        _cameraState = new CameraState();
        _modelState = new ModelState(Model);
        
        int bitmapPixelWidth = bitmap.PixelWidth;
        int bitmapPixelHeight = bitmap.PixelHeight;
        
        _matrixManager = new TransformationMatrixManager(_cameraState, _modelState, bitmapPixelWidth, bitmapPixelHeight);
        _vertexTransformer = new VertexTransformer(_matrixManager, bitmapPixelWidth, bitmapPixelHeight);
        _bitmapRenderer = new BitmapRenderer(bitmap);
        _faceRenderer = new FaceRenderer(_bitmapRenderer, _cameraState);
        _mouseHandler = new MouseInteractionHandler(_cameraState, _matrixManager);
    }

    public void Render()
    {
        ExecuteRendering();
    }

    private void ExecuteRendering()
    {
        try
        {
            RenderModel();
        }
        catch (Exception exception)
        {
            HandleRenderingError(exception);
        }
    }

    private void RenderModel()
    {
        if (HasModelData())
        {
            PrepareForRendering();
            
            PerformRenderingOperations();
        }
    }

    private bool HasModelData()
    {
        return Model.Vertices.Count > 0 && Model.Faces.Count > 0;
    }

    private void PrepareForRendering()
    {
        _modelState.MarkAsDirty();
        
        _modelState.CalculateModelAdjustment();
    }

    private void PerformRenderingOperations()
    {
        _bitmapRenderer.BeginDrawing();
        
        try
        {
            DrawModelInCurrentStateOnScreen();
        }
        finally
        {
            _bitmapRenderer.EndDrawing();
        }
    }

    private void DrawModelInCurrentStateOnScreen()
    {
        UpdateScreenState();

        ReadOnlyMemory<VertexData> vertices = _vertexTransformer.TransformVertices(Model);
        
        _faceRenderer.RenderFaces(Model.Faces, vertices);
    }

    private void UpdateScreenState()
    {
        _bitmapRenderer.ClearBitmap();
            
        _matrixManager.UpdateMatricesIfNeeded();
            
    }

    private static void HandleRenderingError(Exception exception)
    {
        Console.WriteLine($"Render error: {exception.Message}");
    }

    public void OnMouseDown(Point position)
    {
        _mouseHandler.HandleMouseDown(position);
    }

    public void OnMouseMove(Point position)
    {
        _mouseHandler.HandleMouseMove(position);
        Render();
    }

    public void OnMouseUp()
    {
        _mouseHandler.HandleMouseUp();
    }

    public void OnMouseWheel(int delta)
    {
        _mouseHandler.HandleMouseWheel(delta);
        Render();
    }

    public void ZoomIn(float amount)
    {
        _cameraState.ModifyZoom(amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void ZoomOut(float amount)
    {
        _cameraState.ModifyZoom(-amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void MoveUp(float amount)
    {
        _cameraState.ModifyOffsetY(amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void MoveDown(float amount)
    {
        _cameraState.ModifyOffsetY(-amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void MoveLeft(float amount)
    {
        _cameraState.ModifyOffsetX(amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void MoveRight(float amount)
    {
        _cameraState.ModifyOffsetX(-amount);
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }

    public void ResetView()
    {
        _cameraState.ResetToDefault();
        _matrixManager.MarkMatricesAsDirty();
        Render();
    }
}