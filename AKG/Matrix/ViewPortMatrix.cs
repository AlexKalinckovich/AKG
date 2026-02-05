// ViewportMatrix.cs

using System.Numerics;

namespace AKG.Matrix
{
    public static class ViewportMatrix
    {
        public static Matrix4x4 Create(int width, int height)
        {
            // Преобразует из NDC [-1, 1] в экранные координаты [0, width-1] x [0, height-1]
            // с инверсией Y (экранные координаты идут сверху вниз)
            float scaleX = width / 2f;
            float scaleY = height / 2f;
            
            return new Matrix4x4(
                scaleX, 0, 0, scaleX,      // X: [-1,1] -> [0,width]
                0, -scaleY, 0, scaleY,     // Y: [1,-1] -> [0,height] (инверсия)
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }
    }
}