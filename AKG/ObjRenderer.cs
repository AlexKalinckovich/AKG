using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;
using AKG.Core.Model;
using AKG.View;

namespace AKG
{
    public class ObjRenderer
    {
        private readonly ObjModel _model;
        private readonly WriteableBitmap _bitmap;
        private readonly Rasterizer _rasterizer;
        private readonly Camera _camera;
        
        // Состояние мыши для вращения
        private Point? _lastMousePosition;
        private bool _isRotating = false;
        
        public ObjRenderer(ObjModel model, WriteableBitmap bitmap)
        {
            _model = model;
            _bitmap = bitmap;
            _camera = new Camera();
            _rasterizer = new Rasterizer(bitmap, _camera);
        }
        
        public void Render()
        {
            _rasterizer.Render(_model);
        }
        
        // Управление камерой
        public void OnMouseDown(Point position)
        {
            _lastMousePosition = position;
            _isRotating = true;
        }
        
        public void OnMouseMove(Point position)
        {
            if (!_isRotating || _lastMousePosition == null) return;
            
            float deltaX = (float)(position.X - _lastMousePosition.Value.X) * 0.01f;
            float deltaY = (float)(position.Y - _lastMousePosition.Value.Y) * 0.01f;
            
            _camera.Rotate(deltaX, deltaY);
            _lastMousePosition = position;
            
            Render();
        }
        
        public void OnMouseUp()
        {
            _isRotating = false;
            _lastMousePosition = null;
        }
        
        public void OnMouseWheel(int delta)
        {
            float zoomStep = delta > 0 ? -0.5f : 0.5f;
            _camera.Zoom(zoomStep);
            Render();
        }
        
        public void MoveUp(float step)
        {
            _camera.Pan(0, step);
            Render();
        }
        
        public void MoveDown(float step)
        {
            _camera.Pan(0, -step);
            Render();
        }
        
        public void MoveLeft(float step)
        {
            _camera.Pan(step, 0);
            Render();
        }
        
        public void MoveRight(float step)
        {
            _camera.Pan(-step, 0);
            Render();
        }
        
        public void ZoomIn(float step)
        {
            _camera.Zoom(-step);
            Render();
        }
        
        public void ZoomOut(float step)
        {
            _camera.Zoom(step);
            Render();
        }
        
        public void ResetView()
        {
            _camera.Reset();
            Render();
        }
        
        public void ForceModelAdjustment()
        {
            // Центрируем модель
            if (_model.Vertices.Count > 0)
            {
                // Вычисляем центроид модели
                Vector4 centroid = Vector4.Zero;
                foreach (var vertex in _model.Vertices)
                {
                    centroid += vertex;
                }
                centroid /= _model.Vertices.Count;
                
                // Можно было бы сместить модель, но у нас уже есть камера
                // Вместо этого настраиваем камеру
                _camera.Target = new Vector3(centroid.X, centroid.Y, centroid.Z);
                _camera.UpdateViewMatrix();
            }
            
            Render();
        }
    }
}