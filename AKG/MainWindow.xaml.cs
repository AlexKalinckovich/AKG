using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ACG.Model;
using AKG.Facade;

namespace AKG;

public partial class MainWindow : Window
{
    private readonly ObjParserFacade _objParserFacade;
    private readonly ObjRenderer _renderer;
    private readonly ObjModel _currentObjModel;
    
    public MainWindow()
    {
        InitializeComponent();
        
        WriteableBitmap wb = new WriteableBitmap(800, 600, 96, 96, PixelFormats.Bgra32, null);
        ImgDisplay.Source = wb;

        _currentObjModel = new ObjModel();
        _objParserFacade = new ObjParserFacade();
        _renderer = new ObjRenderer(_currentObjModel, wb);

        CreateIcosahedron();
        _renderer.Render();
        
        Focus();
    }

    private void ImagePanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        System.Windows.Point pos = e.GetPosition(ImgDisplay);
        _renderer.OnMouseDown(pos);
    }

    private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point pos = e.GetPosition(ImgDisplay);
            _renderer.OnMouseMove(pos);
        }
    }

    private void ImagePanel_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _renderer.OnMouseUp();
    }

    private void ImagePanel_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        _renderer.OnMouseWheel(e.Delta);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        float moveStep = 0.2f;
        float zoomStep = 0.5f;
        
        switch (e.Key)
        {
            case Key.W:
                _renderer.MoveUp(moveStep);
                break;
            case Key.S:
                _renderer.MoveDown(moveStep);
                break;
            case Key.A:
                _renderer.MoveLeft(moveStep);
                break;
            case Key.D:
                _renderer.MoveRight(moveStep);
                break;
            case Key.Add:
            case Key.OemPlus:
                _renderer.ZoomIn(zoomStep);
                break;
            case Key.Subtract:
            case Key.OemMinus:
                _renderer.ZoomOut(zoomStep);
                break;
            case Key.R:
                _renderer.ResetView();
                break;
            case Key.Escape:
                _renderer.ResetView();
                break;
        }
    }

    private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
    {
        _renderer.ZoomIn(0.5f);
    }

    private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
    {
        _renderer.ZoomOut(0.5f);
    }

    private void BtnUp_Click(object sender, RoutedEventArgs e)
    {
        _renderer.MoveUp(0.2f);
    }

    private void BtnDown_Click(object sender, RoutedEventArgs e)
    {
        _renderer.MoveDown(0.2f);
    }

    private void BtnLeft_Click(object sender, RoutedEventArgs e)
    {
        _renderer.MoveLeft(0.2f);
    }

    private void BtnRight_Click(object sender, RoutedEventArgs e)
    {
        _renderer.MoveRight(0.2f);
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        _renderer.ResetView();
    }

    private async void LoadFile_OnClick(object sender, RoutedEventArgs e)
    {
        await LoadRealObjFile();
    }
    
    private void CreateIcosahedron()
    {
        _currentObjModel.Clear();
        
        float t = (1f + (float)Math.Sqrt(5)) / 2f;
        
        List<Vector4> vertices = new List<Vector4>
        {
            new Vector4(-1,  t,  0, 1),
            new Vector4( 1,  t,  0, 1),
            new Vector4(-1, -t,  0, 1),
            new Vector4( 1, -t,  0, 1),
            
            new Vector4( 0, -1,  t, 1),
            new Vector4( 0,  1,  t, 1),
            new Vector4( 0, -1, -t, 1),
            new Vector4( 0,  1, -t, 1),
            
            new Vector4( t,  0, -1, 1),
            new Vector4( t,  0,  1, 1),
            new Vector4(-t,  0, -1, 1),
            new Vector4(-t,  0,  1, 1)
        };
        
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector4 v = vertices[i];
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            vertices[i] = new Vector4(v.X/length, v.Y/length, v.Z/length, 1);
        }
        
        List<FaceIndices[]> faces = new List<FaceIndices[]>
        {
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(12, 0, 0), new FaceIndices(6, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(6, 0, 0), new FaceIndices(2, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(2, 0, 0), new FaceIndices(8, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(8, 0, 0), new FaceIndices(11, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(11, 0, 0), new FaceIndices(12, 0, 0) },
            
            new[] { new FaceIndices(2, 0, 0), new FaceIndices(6, 0, 0), new FaceIndices(10, 0, 0) },
            new[] { new FaceIndices(6, 0, 0), new FaceIndices(12, 0, 0), new FaceIndices(5, 0, 0) },
            new[] { new FaceIndices(12, 0, 0), new FaceIndices(11, 0, 0), new FaceIndices(3, 0, 0) },
            new[] { new FaceIndices(11, 0, 0), new FaceIndices(8, 0, 0), new FaceIndices(7, 0, 0) },
            new[] { new FaceIndices(8, 0, 0), new FaceIndices(2, 0, 0), new FaceIndices(9, 0, 0) },
            
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(10, 0, 0), new FaceIndices(5, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(5, 0, 0), new FaceIndices(3, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(3, 0, 0), new FaceIndices(7, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(7, 0, 0), new FaceIndices(9, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(9, 0, 0), new FaceIndices(10, 0, 0) },
            
            new[] { new FaceIndices(5, 0, 0), new FaceIndices(10, 0, 0), new FaceIndices(6, 0, 0) },
            new[] { new FaceIndices(3, 0, 0), new FaceIndices(5, 0, 0), new FaceIndices(12, 0, 0) },
            new[] { new FaceIndices(7, 0, 0), new FaceIndices(3, 0, 0), new FaceIndices(11, 0, 0) },
            new[] { new FaceIndices(9, 0, 0), new FaceIndices(7, 0, 0), new FaceIndices(8, 0, 0) },
            new[] { new FaceIndices(10, 0, 0), new FaceIndices(9, 0, 0), new FaceIndices(2, 0, 0) }
        };
        
        _currentObjModel.AddVertices(vertices);
        _currentObjModel.AddFaces(faces);
        
        Console.WriteLine($"Created icosahedron: {_currentObjModel.Vertices.Count} vertices, {_currentObjModel.Faces.Count} faces");
    }

    private async Task LoadRealObjFile()
    {
        _currentObjModel.Clear();
        
        await _objParserFacade.ParseObjModelFromFileAsync(
            onProgress: () =>
            {
                Dispatcher.Invoke(() =>
                {
                    _renderer.ForceModelAdjustment();
                });
            },
            onComplete: () =>
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateModelFromFacade();
                    _renderer.ForceModelAdjustment();
                    Console.WriteLine($"Finished loading: {_currentObjModel.Vertices.Count} vertices, {_currentObjModel.Faces.Count} faces");
                });
            }
        );
    }

    private void UpdateModelFromFacade()
    {
        _currentObjModel.Clear();
        
        IReadOnlyList<Vector4> vertices = _objParserFacade.CurrentObjModel.Vertices;
        IReadOnlyList<FaceIndices[]> faces = _objParserFacade.CurrentObjModel.Faces;
        IReadOnlyList<Vector3> normals = _objParserFacade.CurrentObjModel.Normals;
        IReadOnlyList<Vector2> textureCoords = _objParserFacade.CurrentObjModel.TextureCoords;
        
        _currentObjModel.AddVertices(vertices);
        _currentObjModel.AddFaces(faces);
        _currentObjModel.AddNormals(normals);
        _currentObjModel.AddTextureCoords(textureCoords);
    }
}