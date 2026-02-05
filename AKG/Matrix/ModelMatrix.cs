using System.Numerics;

namespace AKG.Matrix;

public class ModelMatrix
{
    private readonly Matrix4x4 _matrix;
    
    public static Matrix4x4 of(int translateX, int translateY, int translateZ,
        int scaleX, int scaleY, int scaleZ, int angle)
    {
        Matrix4x4 translationMatrix = GetTranslationMatrix(translateX, translateY, translateZ);
        Matrix4x4 scaleMatrix = GetScaleMatrix(scaleX, scaleY, scaleZ);
        Matrix4x4 rotationMatrix = GetRotationMatrix(angle);
        
        return translationMatrix * rotationMatrix * scaleMatrix;
    }
    

    private static Matrix4x4 GetTranslationMatrix(int translateX, int translateY, int translateZ)
    {
        // [1 0 0 Tx]
        // [0 1 0 Ty]
        // [0 0 1 Tz]
        // [0 0 0 1]
        return new Matrix4x4(
            1, 0, 0, translateX,
            0, 1, 0, translateY,
            0, 0, 1, translateZ,
            0, 0, 0, 1
        );
    }

    private static Matrix4x4 GetScaleMatrix(int scaleX, int scaleY, int scaleZ)
    {
        // [Sx 0 0 0]
        // [0 Sy 0 0]
        // [0 0 Sz 0]
        // [0 0 0 1]
        return new Matrix4x4(
            scaleX, 0, 0, 0,
            0, scaleY, 0, 0,
            0, 0, scaleZ, 0,
            0, 0, 0, 1
        );
    }

    private static Matrix4x4 GetRotationMatrix(int angle)
    {
        double radians = angle * Math.PI / 180.0;
        double sin = Math.Sin(radians);
        double cos = Math.Cos(radians);
        
        // [1    0    0 0]
        // [0  cos -sin 0]
        // [0  sin  cos 0]
        // [0    0    0 1]
        return new Matrix4x4(
            1, 0, 0, 0,
            0, (float)cos, (float)-sin, 0,
            0, (float)sin, (float)cos, 0,
            0, 0, 0, 1
        );
    }
    
    private static Matrix4x4 GetRotationMatrixY(int angle)
    {
        double radians = angle * Math.PI / 180.0;
        double sin = Math.Sin(radians);
        double cos = Math.Cos(radians);
        
        // [cos 0 sin 0]
        // [0   1 0   0]
        // [-sin 0 cos 0]
        // [0    0 0   1]
        return new Matrix4x4(
            (float)cos, 0, (float)sin, 0,
            0, 1, 0, 0,
            (float)-sin, 0, (float)cos, 0,
            0, 0, 0, 1
        );
    }
    
    private static Matrix4x4 GetRotationMatrixZ(int angle)
    {
        double radians = angle * Math.PI / 180.0;
        double sin = Math.Sin(radians);
        double cos = Math.Cos(radians);
        
        // [cos -sin 0 0]
        // [sin  cos 0 0]
        // [0    0   1 0]
        // [0    0   0 1]
        return new Matrix4x4(
            (float)cos, (float)-sin, 0, 0,
            (float)sin, (float)cos, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }
}