using System.Numerics;

namespace AKG.Matrix;

public static class ViewMatrix
{
    public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 zAxis = Vector3.Normalize(eye - target);
        
        Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
        
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
        
        float dotX = Vector3.Dot(xAxis, eye);
        
        float dotY = Vector3.Dot(yAxis, eye);
        
        float dotZ = Vector3.Dot(zAxis, eye);

        return new Matrix4x4(
            xAxis.X,  xAxis.Y,  xAxis.Z,  -dotX,
            yAxis.X,  yAxis.Y,  yAxis.Z,  -dotY,
            zAxis.X,  zAxis.Y,  zAxis.Z,  -dotZ,
            0,        0,        0,        1
        );
    }
    
    
    public static Matrix4x4 LookAtAlternative(Vector3 eye, Vector3 target, Vector3 up)
    {
        
        Vector3 zAxis = Vector3.Normalize(target - eye);
        
        
        Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
        
        
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
        
        
        
        return new Matrix4x4(
            xAxis.X,  yAxis.X,  zAxis.X,  0,
            xAxis.Y,  yAxis.Y,  zAxis.Y,  0,
            xAxis.Z,  yAxis.Z,  zAxis.Z,  0,
            -Vector3.Dot(xAxis, eye), -Vector3.Dot(yAxis, eye), -Vector3.Dot(zAxis, eye), 1
        );
    }
    
    
    public static Matrix4x4 CreateDefault()
    {
        
        Vector3 eye = new Vector3(0, 5, 10);    
        Vector3 target = new Vector3(0, 0, 0);  
        Vector3 up = new Vector3(0, 1, 0);      
        
        return LookAt(eye, target, up);
    }
    
    
    public static Matrix4x4 CreateOrbitalCamera(float angleDegrees, float distance, float height)
    {
        
        float angleRad = angleDegrees * (float)Math.PI / 180.0f;
        
        
        Vector3 eye = new Vector3(
            (float)Math.Sin(angleRad) * distance,
            height,
            (float)Math.Cos(angleRad) * distance
        );
        
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 up = new Vector3(0, 1, 0);
        
        return LookAt(eye, target, up);
    }
}
