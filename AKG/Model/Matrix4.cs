using System.Numerics;
using System.Runtime.CompilerServices;

namespace AKG.Model;

public unsafe struct Matrix4
{
    private fixed float _m[16];
    
    public float this[int row, int col]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _m[row * 4 + col];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _m[row * 4 + col] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 Identity()
    {
        Matrix4 m = new Matrix4();
        m[0, 0] = 1; m[1, 1] = 1; m[2, 2] = 1; m[3, 3] = 1;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 operator *(Matrix4 a, Matrix4 b)
    {
        Matrix4 result = new Matrix4();
        
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                float sum = 0;
                for (int k = 0; k < 4; k++)
                {
                    sum += a[row, k] * b[k, col];
                }
                result[row, col] = sum;
            }
        }
        
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4 TransformPoint(Vector4 point)
    {
        if (Vector.IsHardwareAccelerated)
        {
            Vector4 row0 = new Vector4(this[0, 0], this[0, 1], this[0, 2], this[0, 3]);
            Vector4 row1 = new Vector4(this[1, 0], this[1, 1], this[1, 2], this[1, 3]);
            Vector4 row2 = new Vector4(this[2, 0], this[2, 1], this[2, 2], this[2, 3]);
            Vector4 row3 = new Vector4(this[3, 0], this[3, 1], this[3, 2], this[3, 3]);
            
            
            float x = Vector4.Dot(row0, point);
            float y = Vector4.Dot(row1, point);
            float z = Vector4.Dot(row2, point);
            float w = Vector4.Dot(row3, point);
            
            return new Vector4(x, y, z, w);
        }
        else
        {
            
            float x = this[0, 0] * point.X + this[0, 1] * point.Y + this[0, 2] * point.Z + this[0, 3] * point.W;
            float y = this[1, 0] * point.X + this[1, 1] * point.Y + this[1, 2] * point.Z + this[1, 3] * point.W;
            float z = this[2, 0] * point.X + this[2, 1] * point.Y + this[2, 2] * point.Z + this[2, 3] * point.W;
            float w = this[3, 0] * point.X + this[3, 1] * point.Y + this[3, 2] * point.Z + this[3, 3] * point.W;
            
            
            return new Vector4(x, y, z, w);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 RotateX(float radians)
    {
        float c = (float)Math.Cos(radians);
        float s = (float)Math.Sin(radians);
        
        Matrix4 m = Identity();
        
        m[1, 1] = c; m[1, 2] = -s;
        m[2, 1] = s; m[2, 2] = c;
        
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 RotateY(float radians)
    {
        float c = (float)Math.Cos(radians);
        float s = (float)Math.Sin(radians);
        Matrix4 m = Identity();
        m[0, 0] = c; m[0, 2] = s;
        m[2, 0] = -s; m[2, 2] = c;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 RotateZ(float radians)
    {
        float c = (float)Math.Cos(radians);
        float s = (float)Math.Sin(radians);
        Matrix4 m = Identity();
        m[0, 0] = c; m[0, 1] = -s;
        m[1, 0] = s; m[1, 1] = c;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 Translate(float x, float y, float z)
    {
        Matrix4 m = Identity();
        m[0, 3] = x;
        m[1, 3] = y;
        m[2, 3] = z;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 Scale(float sx, float sy, float sz)
    {
        Matrix4 m = Identity();
        m[0, 0] = sx;
        m[1, 1] = sy;
        m[2, 2] = sz;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 zAxis = Vector3.Normalize(eye - target);
        Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

        Matrix4 m = Identity();
        m[0, 0] = xAxis.X; m[0, 1] = xAxis.Y; m[0, 2] = xAxis.Z; m[0, 3] = -Vector3.Dot(xAxis, eye);
        m[1, 0] = yAxis.X; m[1, 1] = yAxis.Y; m[1, 2] = yAxis.Z; m[1, 3] = -Vector3.Dot(yAxis, eye);
        m[2, 0] = zAxis.X; m[2, 1] = zAxis.Y; m[2, 2] = zAxis.Z; m[2, 3] = -Vector3.Dot(zAxis, eye);
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 Perspective(float fovRadians, float aspect, float znear, float zfar)
    {
        float f = 1f / (float)Math.Tan(fovRadians / 2);
        Matrix4 m = new Matrix4();
        
        m[0, 0] = f / aspect;
        m[1, 1] = f;
        m[2, 2] = (zfar + znear) / (znear - zfar);
        m[2, 3] = (2 * zfar * znear) / (znear - zfar);
        m[3, 2] = -1;
        m[3, 3] = 0;
        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4 Orthographic(float width, float height, float znear, float zfar)
    {
        Matrix4 m = Identity();
        m[0, 0] = 2f / width;
        m[1, 1] = 2f / height;
        m[2, 2] = 1f / (zfar - znear);
        m[2, 3] = znear / (znear - zfar);
        return m;
    }
}