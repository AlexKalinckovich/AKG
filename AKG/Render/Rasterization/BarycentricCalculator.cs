using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Render.Rasterization;

public static class BarycentricCalculator
{
    private const float Equal = 1f / 3f;
    public static bool Logs { get; set; } = false;

    public static TriangleWeight ComputeBarycentricCoordinates(Point pointInTriangle, Triangle triangle)
    {
        double triangleArea = CalculateTriangleArea(triangle);
        float weight0 = 0f;
        float weight1 = 0f;
        float weight2 = 0f;

        if (Math.Abs(triangleArea) < RasterizationConstants.BarycentricEpsilon)
        {
            weight0 = Equal;
            weight1 = Equal;
            weight2 = Equal;
        }
        else
        {
            Point point0 = triangle.Vertex0.ScreenPoint;
            Point point1 = triangle.Vertex1.ScreenPoint;
            Point point2 = triangle.Vertex2.ScreenPoint;

            weight0 = (float)CalculateWeight(point1, point2, pointInTriangle, triangleArea);
            weight1 = (float)CalculateWeight(point2, point0, pointInTriangle, triangleArea);
            weight2 = 1 - weight0 - weight1;
            if (weight2 < 0)
            {
                weight2 = 0;
            }
        }

        return new TriangleWeight(weight0, weight1, weight2);
    }

    
    public static Vector2 ComputePerspectiveCorrectUv(Triangle triangle, TriangleWeight screenWeights)
    {
        Vector4 clip0 = triangle.Vertex0.ClipPosition;
        Vector4 clip1 = triangle.Vertex1.ClipPosition;
        Vector4 clip2 = triangle.Vertex2.ClipPosition;
        
        float w0 = clip0.W;
        float w1 = clip1.W;
        float w2 = clip2.W;
        
        Vector2 uv0 = triangle.Vertex0.UV;
        Vector2 uv1 = triangle.Vertex1.UV;
        Vector2 uv2 = triangle.Vertex2.UV;

        float invW0 = 1.0f / w0;
        float invW1 = 1.0f / w1;
        float invW2 = 1.0f / w2;
        
        float total = screenWeights.Weight0 * invW0 + 
                      screenWeights.Weight1 * invW1 + 
                      screenWeights.Weight2 * invW2;
        
        float u = (screenWeights.Weight0 * uv0.X * invW0 + 
                   screenWeights.Weight1 * uv1.X * invW1 + 
                   screenWeights.Weight2 * uv2.X * invW2) / total;
        
        float v = (screenWeights.Weight0 * uv0.Y * invW0 + 
                   screenWeights.Weight1 * uv1.Y * invW1 + 
                   screenWeights.Weight2 * uv2.Y * invW2) / total;
        
        Vector2 result = new Vector2(u, v);
        
        
        return result;
    }

    private static double CalculateTriangleArea(Triangle triangle)
    {
        Point point0 = triangle.Vertex0.ScreenPoint;
        Point point1 = triangle.Vertex1.ScreenPoint;
        Point point2 = triangle.Vertex2.ScreenPoint;

        return (point1.Y - point2.Y) * (point0.X - point2.X) +
               (point2.X - point1.X) * (point0.Y - point2.Y);
    }

    private static double CalculateWeight(Point pointA, Point pointB, Point pointC, double area)
    {
        double subTriangleArea = (pointA.Y - pointB.Y) * (pointC.X - pointB.X) +
                                 (pointB.X - pointA.X) * (pointC.Y - pointB.Y);

        return subTriangleArea / area;
    }

    public static bool IsPointInTriangle(Point possibleInTrianglePoint, Triangle triangle)
    {
        
        Point point0 = triangle.Vertex0.ScreenPoint;
        Point point1 = triangle.Vertex1.ScreenPoint;
        Point point2 = triangle.Vertex2.ScreenPoint;

        double sign1 = CalculateCrossProduct(point0, point1, possibleInTrianglePoint);
        double sign2 = CalculateCrossProduct(point1, point2, possibleInTrianglePoint);
        double sign3 = CalculateCrossProduct(point2, point0, possibleInTrianglePoint);

        bool allNonNegative = sign1 >= -RasterizationConstants.BarycentricEpsilon &&
                              sign2 >= -RasterizationConstants.BarycentricEpsilon && 
                              sign3 >= -RasterizationConstants.BarycentricEpsilon;
        
        bool allNonPositive = sign1 <= RasterizationConstants.BarycentricEpsilon &&
                              sign2 <= RasterizationConstants.BarycentricEpsilon && 
                              sign3 <= RasterizationConstants.BarycentricEpsilon;
        
        return allNonNegative || allNonPositive;
    }

    private static double CalculateCrossProduct(Point pointA, Point pointB, Point pointC)
    {
        return (pointB.X - pointA.X) * (pointC.Y - pointA.Y) -
               (pointB.Y - pointA.Y) * (pointC.X - pointA.X);
    }
}