using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AKG.Core.Model;

namespace AKG.View
{
    public class Rasterizer
    {
        private readonly WriteableBitmap _bitmap;
        private readonly Camera _camera;
        
        // Параметры отображения
        private Vector2 _screenCenter;
        private float _scale = 100.0f;
        
        // Буфер глубины
        private float[,] _depthBuffer;
        
        public Rasterizer(WriteableBitmap bitmap, Camera camera)
        {
            _bitmap = bitmap;
            _camera = camera;
            _screenCenter = new Vector2(bitmap.PixelWidth / 2, bitmap.PixelHeight / 2);
            
            InitializeDepthBuffer();
        }
        
        private void InitializeDepthBuffer()
        {
            _depthBuffer = new float[_bitmap.PixelWidth, _bitmap.PixelHeight];
            ClearDepthBuffer();
        }
        
        private void ClearDepthBuffer()
        {
            for (int y = 0; y < _bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < _bitmap.PixelWidth; x++)
                {
                    _depthBuffer[x, y] = float.MaxValue;
                }
            }
        }
        
        public void Render(ObjModel model)
        {
            // Очищаем буферы
            ClearBitmap();
            ClearDepthBuffer();
            
            // Рендерим каждую грань
            foreach (var face in model.Faces)
            {
                if (face.Length < 3) continue;
                
                RenderFace(model, face);
            }
        }
        
        private void RenderFace(ObjModel model, FaceIndices[] face)
        {
            // Преобразуем вершины грани в экранные координаты
            var screenPoints = new Vector2[face.Length];
            var depths = new float[face.Length];
            
            for (int i = 0; i < face.Length; i++)
            {
                var vertex = GetTransformedVertex(model, face[i]);
                screenPoints[i] = ProjectToScreen(vertex, out depths[i]);
            }
            
            // Рендерим полигон (треугольник или многоугольник)
            if (face.Length == 3)
            {
                RenderTriangle(screenPoints, depths);
            }
            else
            {
                // Разбиваем многоугольник на треугольники
                for (int i = 1; i < face.Length - 1; i++)
                {
                    var trianglePoints = new Vector2[] 
                    { 
                        screenPoints[0], 
                        screenPoints[i], 
                        screenPoints[i + 1] 
                    };
                    var triangleDepths = new float[]
                    {
                        depths[0],
                        depths[i],
                        depths[i + 1]
                    };
                    
                    RenderTriangle(trianglePoints, triangleDepths);
                }
            }
        }
        
        private Vector4 GetTransformedVertex(ObjModel model, FaceIndices indices)
        {
            // Берем вершину из модели
            Vector4 vertex = model.Vertices[indices.VertexIndex];
            
            // Применяем матрицу вида
            vertex = Vector4.Transform(vertex, _camera.ViewMatrix);
            
            // Применяем матрицу проекции
            vertex = Vector4.Transform(vertex, _camera.ProjectionMatrix);
            
            // Перспективное деление
            if (vertex.W != 0)
            {
                vertex.X /= vertex.W;
                vertex.Y /= vertex.W;
                vertex.Z /= vertex.W;
            }
            
            return vertex;
        }
        
        private Vector2 ProjectToScreen(Vector4 vertex, out float depth)
        {
            depth = vertex.Z;
            
            // Преобразуем из нормализованных координат устройства (-1..1) в экранные координаты
            float screenX = (vertex.X + 1.0f) * 0.5f * _bitmap.PixelWidth;
            float screenY = (1.0f - vertex.Y) * 0.5f * _bitmap.PixelHeight;
            
            // Масштабируем и центрируем
            screenX = _screenCenter.X + (screenX - _screenCenter.X) * _scale;
            screenY = _screenCenter.Y + (screenY - _screenCenter.Y) * _scale;
            
            return new Vector2(screenX, screenY);
        }
        
        private void RenderTriangle(Vector2[] points, float[] depths)
        {
            if (points.Length != 3) return;
            
            // Находим ограничивающий прямоугольник треугольника
            float minX = Math.Min(points[0].X, Math.Min(points[1].X, points[2].X));
            float maxX = Math.Max(points[0].X, Math.Max(points[1].X, points[2].X));
            float minY = Math.Min(points[0].Y, Math.Min(points[1].Y, points[2].Y));
            float maxY = Math.Max(points[0].Y, Math.Max(points[1].Y, points[2].Y));
            
            // Ограничиваем экраном
            int startX = Math.Max(0, (int)Math.Floor(minX));
            int endX = Math.Min(_bitmap.PixelWidth - 1, (int)Math.Ceiling(maxX));
            int startY = Math.Max(0, (int)Math.Floor(minY));
            int endY = Math.Min(_bitmap.PixelHeight - 1, (int)Math.Ceiling(maxY));
            
            // Баррицентрические координаты
            float area = EdgeFunction(points[0], points[1], points[2]);
            
            if (area == 0) return; // Вырожденный треугольник
            
            // Проверяем каждую точку в ограничивающем прямоугольнике
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    
                    float w0 = EdgeFunction(points[1], points[2], p);
                    float w1 = EdgeFunction(points[2], points[0], p);
                    float w2 = EdgeFunction(points[0], points[1], p);
                    
                    // Если точка внутри треугольника
                    if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                    {
                        // Баррицентрические координаты
                        float alpha = w0 / area;
                        float beta = w1 / area;
                        float gamma = w2 / area;
                        
                        // Интерполируем глубину
                        float depth = alpha * depths[0] + beta * depths[1] + gamma * depths[2];
                        
                        // Проверка глубины
                        if (depth < _depthBuffer[x, y])
                        {
                            _depthBuffer[x, y] = depth;
                            SetPixel(x, y, Colors.White);
                        }
                    }
                }
            }
            
            // Рисуем контуры треугольника
            DrawLine(points[0], points[1], Colors.Red);
            DrawLine(points[1], points[2], Colors.Red);
            DrawLine(points[2], points[0], Colors.Red);
        }
        
        private float EdgeFunction(Vector2 a, Vector2 b, Vector2 c)
        {
            return (c.X - a.X) * (b.Y - a.Y) - (c.Y - a.Y) * (b.X - a.X);
        }
        
        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            int x0 = (int)start.X;
            int y0 = (int)start.Y;
            int x1 = (int)end.X;
            int y1 = (int)end.Y;
            
            // Алгоритм Брезенхема
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            
            while (true)
            {
                if (x0 >= 0 && x0 < _bitmap.PixelWidth && y0 >= 0 && y0 < _bitmap.PixelHeight)
                {
                    SetPixel(x0, y0, color);
                }
                
                if (x0 == x1 && y0 == y1) break;
                
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
        
        private void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= _bitmap.PixelWidth || y < 0 || y >= _bitmap.PixelHeight)
                return;
            
            try
            {
                _bitmap.Lock();
                
                unsafe
                {
                    byte* buffer = (byte*)_bitmap.BackBuffer;
                    int stride = _bitmap.BackBufferStride;
                    int index = y * stride + x * 4;
                    
                    buffer[index] = color.B;     // Blue
                    buffer[index + 1] = color.G; // Green
                    buffer[index + 2] = color.R; // Red
                    buffer[index + 3] = color.A; // Alpha
                }
                
                _bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                _bitmap.Unlock();
            }
        }
        
        private void ClearBitmap()
        {
            try
            {
                _bitmap.Lock();
                
                unsafe
                {
                    byte* buffer = (byte*)_bitmap.BackBuffer;
                    int stride = _bitmap.BackBufferStride;
                    int bytesPerPixel = 4;
                    
                    for (int y = 0; y < _bitmap.PixelHeight; y++)
                    {
                        byte* row = buffer + y * stride;
                        for (int x = 0; x < _bitmap.PixelWidth; x++)
                        {
                            row[x * bytesPerPixel] = 0;     // Blue
                            row[x * bytesPerPixel + 1] = 0; // Green
                            row[x * bytesPerPixel + 2] = 0; // Red
                            row[x * bytesPerPixel + 3] = 255; // Alpha
                        }
                    }
                }
                
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            }
            finally
            {
                _bitmap.Unlock();
            }
        }
    }
}