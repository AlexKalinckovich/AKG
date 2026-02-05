using System.Numerics;

namespace AKG.Matrix;

public static class ProjectionMatrix
{
    
    public static Matrix4x4 CreatePerspective(float width, float height, float znear, float zfar)
    {
        if (znear <= 0) throw new ArgumentException("znear должно быть больше 0");
        if (zfar <= znear) throw new ArgumentException("zfar должно быть больше znear");
        if (width <= 0) throw new ArgumentException("width должно быть больше 0");
        if (height <= 0) throw new ArgumentException("height должно быть больше 0");
       
        return new Matrix4x4(
            (2 * znear) / width, 0, 0, 0,
            0, (2 * znear) / height, 0, 0,
            0, 0, zfar / (znear - zfar), (znear * zfar) / (znear - zfar),
            0, 0, -1, 0
        );
    }
    
    
    public static Matrix4x4 CreatePerspectiveFov(float fovY, float aspect, float znear, float zfar)
    {
        
        if (fovY <= 0 || fovY >= Math.PI) 
            throw new ArgumentException("FOV должен быть в диапазоне (0, π)");
        if (aspect <= 0) throw new ArgumentException("aspect должно быть больше 0");
        if (znear <= 0) throw new ArgumentException("znear должно быть больше 0");
        if (zfar <= znear) throw new ArgumentException("zfar должно быть больше znear");
        
        
        
        float tanHalfFov = (float)Math.Tan(fovY / 2);
        
        
        float height = 2 * znear * tanHalfFov;
        
        float width = height * aspect;
        
        
        return CreatePerspective(width, height, znear, zfar);
    }
    
    public static Matrix4x4 CreatePerspectiveFovDegrees(float fovYDegrees, float aspect, float znear, float zfar)
    {
        float fovYRadians = fovYDegrees * (float)Math.PI / 180.0f;
        return CreatePerspectiveFov(fovYRadians, aspect, znear, zfar);
    }
    
    public static Matrix4x4 CreateOrthographic(float width, float height, float znear, float zfar)
    {
        if (znear >= zfar) throw new ArgumentException("znear должно быть меньше zfar");
        if (width <= 0) throw new ArgumentException("width должно быть больше 0");
        if (height <= 0) throw new ArgumentException("height должно быть больше 0");
        
        return new Matrix4x4(
            2 / width, 0, 0, 0,
            0, 2 / height, 0, 0,
            0, 0, 1 / (znear - zfar), znear / (znear - zfar),
            0, 0, 0, 1
        );
    }
    
    
    public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float znear, float zfar)
    {
        
        if (left >= right) throw new ArgumentException("left должно быть меньше right");
        if (bottom >= top) throw new ArgumentException("bottom должно быть меньше top");
        if (znear >= zfar) throw new ArgumentException("znear должно быть меньше zfar");
        
        float width = right - left;
        float height = top - bottom;
        
        return new Matrix4x4(
            2 / width, 0, 0, -(right + left) / width,
            0, 2 / height, 0, -(top + bottom) / height,
            0, 0, 1 / (znear - zfar), znear / (znear - zfar),
            0, 0, 0, 1
        );
    }
    
    public static Matrix4x4 CreateViewport(float xMin, float yMin, float width, float height)
    {
        if (width <= 0) throw new ArgumentException("width должно быть больше 0");
        if (height <= 0) throw new ArgumentException("height должно быть больше 0");
        
        
        return new Matrix4x4(
            width / 2, 0, 0, xMin + width / 2,
            0, -height / 2, 0, yMin + height / 2,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }
    
    
    
    
    public static Vector4 ApplyProjection(Matrix4x4 projectionMatrix, Vector4 vector)
    {
        
        Vector4 result = Vector4.Transform(vector, projectionMatrix);
        
        
        if (Math.Abs(result.W) > float.Epsilon)
        {
            result.X /= result.W;
            result.Y /= result.W;
            result.Z /= result.W;
            result.W = 1.0f;
        }
        
        return result;
    }
    
    public static bool IsPointVisible(Vector4 pointInClipSpace)
    {
        
        return Math.Abs(pointInClipSpace.X) <= Math.Abs(pointInClipSpace.W) &&
               Math.Abs(pointInClipSpace.Y) <= Math.Abs(pointInClipSpace.W) &&
               pointInClipSpace.Z >= 0 && pointInClipSpace.Z <= Math.Abs(pointInClipSpace.W);
    }
}