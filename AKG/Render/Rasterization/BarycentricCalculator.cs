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