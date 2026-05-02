using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Model.Vertex;
using AKG.Render.Constants;

namespace AKG.Render.Culling;

public static class NearPlaneClipper
{
    

    
    public static int Clip(Triangle inTri, int viewportWidth, int viewportHeight, out Triangle out1, out Triangle out2)
    {
        out1 = default;
        out2 = default;

        
        VertexData[] v = [inTri.Vertex0, inTri.Vertex1, inTri.Vertex2];
        
        
        float[] d =
        [
            v[0].ClipPosition.W - RenderConstants.DefaultZNearClippingPlane,
            v[1].ClipPosition.W - RenderConstants.DefaultZNearClippingPlane,
            v[2].ClipPosition.W - RenderConstants.DefaultZNearClippingPlane
        ];

        int inCount = 0;
        int inIndex = -1;
        int outIndex = -1;

        
        for (int i = 0; i < 3; i++)
        {
            if (d[i] >= 0)
            {
                inCount++;
                inIndex = i; 
            }
            else
            {
                outIndex = i; 
            }
        }

        if (inCount == 0) return 0; 
        if (inCount == 3)
        {
            out1 = inTri; 
            return 1;
        }

        
        if (inCount == 1) 
        {
            VertexData vIn = v[inIndex];
            
            VertexData vOut1 = v[(inIndex + 1) % 3]; 
            VertexData vOut2 = v[(inIndex + 2) % 3];

            VertexData newV1 = LerpVertex(vIn, vOut1, viewportWidth, viewportHeight);
            VertexData newV2 = LerpVertex(vIn, vOut2, viewportWidth, viewportHeight);

            out1 = new Triangle(vIn, newV1, newV2);
            return 1;
        }

        
        if (inCount == 2) 
        {
            VertexData vOut = v[outIndex];
            VertexData vIn1 = v[(outIndex + 1) % 3];
            VertexData vIn2 = v[(outIndex + 2) % 3];

            VertexData newV1 = LerpVertex(vIn1, vOut, viewportWidth, viewportHeight);
            VertexData newV2 = LerpVertex(vIn2, vOut, viewportWidth, viewportHeight);

            out1 = new Triangle(vIn1, vIn2, newV1);
            out2 = new Triangle(vIn2, newV2, newV1);
            return 2;
        }

        return 0;
    }

    
    private static VertexData LerpVertex(VertexData vIn, VertexData vOut, int viewportWidth, int viewportHeight)
    {
        float dIn = vIn.ClipPosition.W - RenderConstants.DefaultZNearClippingPlane;
        float dOut = vOut.ClipPosition.W - RenderConstants.DefaultZNearClippingPlane;
        
        
        float t = dIn / (dIn - dOut);

        Vector3 worldPos = Vector3.Lerp(vIn.WorldPosition, vOut.WorldPosition, t);
        Vector3 viewPos = Vector3.Lerp(vIn.ViewPosition, vOut.ViewPosition, t);
        Vector4 clipPos = Vector4.Lerp(vIn.ClipPosition, vOut.ClipPosition, t);
        float depth = vIn.Depth + (vOut.Depth - vIn.Depth) * t;
        Vector3 normal = Vector3.Normalize(Vector3.Lerp(vIn.Normal, vOut.Normal, t));
        Vector2 uv = Vector2.Lerp(vIn.UV, vOut.UV, t);

        
        float halfWidth = viewportWidth * 0.5f;
        float halfHeight = viewportHeight * 0.5f;
        
        float normX = clipPos.X / clipPos.W;
        float normY = clipPos.Y / clipPos.W;
        
        double screenX = (normX + 1.0) * halfWidth;
        double screenY = (1.0 - normY) * halfHeight;
        Point screenPoint = new Point(screenX, screenY);

        return new VertexData(worldPos, viewPos, clipPos, screenPoint, depth, normal, uv);
    }
}