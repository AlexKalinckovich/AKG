
using System.Numerics;

namespace AKG.View
{
    public class Camera
    {
        // Позиция камеры и целевая точка
        public Vector3 Eye { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        
        // Параметры проекции
        public float FieldOfView { get; set; } = 60.0f; // в градусах
        public float AspectRatio { get; set; } = 4.0f / 3.0f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 1000.0f;
        
        // Матрицы преобразования
        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }
        
        // Для управления
        private float _distance = 10.0f;
        private float _yaw = 0.0f;   // поворот по горизонтали
        private float _pitch = 0.0f; // поворот по вертикали
        
        public Camera()
        {
            // Начальная позиция камеры
            Eye = new Vector3(0, 5, 10);
            Target = Vector3.Zero;
            Up = Vector3.UnitY;
            
            UpdateViewMatrix();
            UpdateProjectionMatrix(800, 600);
        }
        
        public void UpdateViewMatrix()
        {
            // Вычисляем позицию камеры на основе расстояния и углов
            if (_distance <= 0) _distance = 0.1f;
            
            // Сферические координаты
            float x = _distance * (float)Math.Cos(_pitch) * (float)Math.Sin(_yaw);
            float y = _distance * (float)Math.Sin(_pitch);
            float z = _distance * (float)Math.Cos(_pitch) * (float)Math.Cos(_yaw);
            
            Eye = new Vector3(x, y, z);
            
            // Вычисляем матрицу вида по алгоритму LookAt
            Vector3 zAxis = Vector3.Normalize(Eye - Target);
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(Up, zAxis));
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
            
            ViewMatrix = new Matrix4x4(
                xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, Eye),
                yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, Eye),
                zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, Eye),
                0, 0, 0, 1
            );
        }
        
        public void UpdateProjectionMatrix(int width, int height)
        {
            AspectRatio = (float)width / height;
            
            // Перспективная проекция
            float fovRad = FieldOfView * (float)Math.PI / 180.0f;
            float f = 1.0f / (float)Math.Tan(fovRad / 2.0f);
            float range = NearPlane - FarPlane;
            
            ProjectionMatrix = new Matrix4x4(
                f / AspectRatio, 0, 0, 0,
                0, f, 0, 0,
                0, 0, (FarPlane + NearPlane) / range, 2 * FarPlane * NearPlane / range,
                0, 0, -1, 0
            );
        }
        
        // Методы управления камерой
        public void Rotate(float deltaYaw, float deltaPitch)
        {
            _yaw += deltaYaw;
            _pitch += deltaPitch;
            
            // Ограничиваем угол наклона, чтобы не переворачивать камеру
            _pitch = Math.Clamp(_pitch, -(float)Math.PI / 2 + 0.1f, (float)Math.PI / 2 - 0.1f);
            
            UpdateViewMatrix();
        }
        
        public void Zoom(float delta)
        {
            _distance += delta;
            _distance = Math.Clamp(_distance, 1.0f, 100.0f);
            UpdateViewMatrix();
        }
        
        public void Pan(float deltaX, float deltaY)
        {
            // Вычисляем правый и верхний векторы камеры
            Vector3 zAxis = Vector3.Normalize(Eye - Target);
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(Up, zAxis));
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
            
            // Сдвигаем и камеру, и цель
            Vector3 panOffset = xAxis * deltaX + yAxis * deltaY;
            Eye += panOffset;
            Target += panOffset;
            
            UpdateViewMatrix();
        }
        
        public void Reset()
        {
            _distance = 10.0f;
            _yaw = 0.0f;
            _pitch = 0.0f;
            UpdateViewMatrix();
        }
    }
}