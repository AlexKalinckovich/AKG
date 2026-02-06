using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AKG.Core.Model;
using AKG.Facade;
using AKG.Handlers;
using AKG.Matrix;
using AKG.Render;

namespace AKG;

public partial class MainWindow : Window
{
    private readonly ObjParserFacade _objParserFacade;
    private readonly ObjRenderer _renderer;
    private readonly ModelMoveHandlerFacade _modelMoveHandlerFacade;
    private readonly MouseEventHandlerFacade _mouseEventHandlerFacade;
    
    public MainWindow()
    {
        InitializeComponent();
        
        WriteableBitmap wb = new WriteableBitmap(1280, 600, 96, 96, PixelFormats.Bgra32, null);
        ImgDisplay.Source = wb;

        _objParserFacade = new ObjParserFacade();
        
        _renderer = new ObjRenderer (wb);
        _renderer.Model = TestModelGenerator.CreateIcosahedron();

        _modelMoveHandlerFacade = new ModelMoveHandlerFacade(_renderer);
        _mouseEventHandlerFacade = new MouseEventHandlerFacade(_renderer);
        
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
    
    private async Task LoadRealObjFile()
    {
        ObjModel parsedModel = await _objParserFacade.ParseObjModelFromFileAsync();
        _renderer.Model = parsedModel;
        _renderer.Render();
    }
    
}