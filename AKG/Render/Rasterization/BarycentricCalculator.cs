using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Render.Rasterization;

public static class BarycentricCalculator
{
    private const float Equal = 1f / 3f;

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
        }

        return new TriangleWeight(weight0, weight1, weight2);
    }

    public static Vector2 ComputePerspectiveCorrectUv(Triangle triangle, TriangleWeight weights)
    {
        float z0 = triangle.Vertex0.Depth;
        float z1 = triangle.Vertex1.Depth;
        float z2 = triangle.Vertex2.Depth;

        Vector2 uv0 = triangle.Vertex0.UV;
        Vector2 uv1 = triangle.Vertex1.UV;
        Vector2 uv2 = triangle.Vertex2.UV;

        float invZ0 = 1.0f / Math.Max(z0, RasterizationConstants.BarycentricEpsilon);
        float invZ1 = 1.0f / Math.Max(z1, RasterizationConstants.BarycentricEpsilon);
        float invZ2 = 1.0f / Math.Max(z2, RasterizationConstants.BarycentricEpsilon);

        Vector2 uv0DivZ = uv0 * invZ0;
        Vector2 uv1DivZ = uv1 * invZ1;
        Vector2 uv2DivZ = uv2 * invZ2;

        Vector2 uvDivZ = weights.Weight0 * uv0DivZ +
                         weights.Weight1 * uv1DivZ +
                         weights.Weight2 * uv2DivZ;

        float invZInterp = weights.Weight0 * invZ0 +
                           weights.Weight1 * invZ1 +
                           weights.Weight2 * invZ2;

        float zInterp = 1.0f / Math.Max(invZInterp, RasterizationConstants.BarycentricEpsilon);

        Vector2 uvCorrected = uvDivZ * zInterp;

        return uvCorrected;
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

        return (sign1 >= 0 && sign2 >= 0 && sign3 >= 0) ||
               (sign1 <= 0 && sign2 <= 0 && sign3 <= 0);
    }

    private static double CalculateCrossProduct(Point pointA, Point pointB, Point pointC)
    {
        return (pointB.X - pointA.X) * (pointC.Y - pointA.Y) -
               (pointB.Y - pointA.Y) * (pointC.X - pointA.X);
    }
}