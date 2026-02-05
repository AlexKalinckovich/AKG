using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AKG.Core.Model;
using AKG.Facade;
using AKG.Handlers;

namespace AKG;

public partial class MainWindow : Window
{
    private readonly ObjParserFacade _objParserFacade;
    private readonly ObjRenderer _renderer;
    private ObjModel _currentObjModel;
    private readonly ModelMoveHandlerFacade _modelMoveHandlerFacade;
    private readonly MouseEventHandlerFacade _mouseEventHandlerFacade;
    
    public MainWindow()
    {
        InitializeComponent();
        
        WriteableBitmap wb = new WriteableBitmap(1280, 600, 96, 96, PixelFormats.Bgra32, null);
        ImgDisplay.Source = wb;

        _currentObjModel = new ObjModel();
        _objParserFacade = new ObjParserFacade();
        _renderer = new ObjRenderer (wb);
        _modelMoveHandlerFacade = new ModelMoveHandlerFacade(_renderer);
        _mouseEventHandlerFacade = new MouseEventHandlerFacade(_renderer);
        
        _renderer.Model = CreateIcosahedron();
        _renderer.Render();
        
        Focus();
    }

    private void ImagePanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Point pos = e.GetPosition(ImgDisplay);
        _mouseEventHandlerFacade.HandleMouseDown(pos);
    }

    private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point pos = e.GetPosition(ImgDisplay);
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
        _modelMoveHandlerFacade.HandleKeyPress(e.Key);
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
        _modelMoveHandlerFacade.HandleUpMove();
    }

    private void BtnDown_Click(object sender, RoutedEventArgs e)
    {
        _modelMoveHandlerFacade.HandleDownMove();
    }

    private void BtnLeft_Click(object sender, RoutedEventArgs e)
    {
        _modelMoveHandlerFacade.HandleLeftMove();
    }

    private void BtnRight_Click(object sender, RoutedEventArgs e)
    {
        _modelMoveHandlerFacade.HandleRightMove();
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        _renderer.ResetView();
    }

    private void LoadFile_OnClick(object sender, RoutedEventArgs e)
    {
        _ = LoadRealObjFile();
    }
    
    private ObjModel CreateIcosahedron()
    {
        ObjModel model = new ObjModel();
        
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
        
        model.AddVertices(vertices);
        model.AddFaces(faces);

        return model;
    }

    private async Task LoadRealObjFile()
    {
        ObjModel parsedModel = await _objParserFacade.ParseObjModelFromFileAsync();
        _renderer.Model = parsedModel;
        _renderer.Render();
    }
    
}