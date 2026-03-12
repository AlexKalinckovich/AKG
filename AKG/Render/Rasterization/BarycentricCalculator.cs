using System.Windows;
using AKG.Model;
using AKG.Render.Constants;

namespace AKG.Render.Rasterization;


public sealed class BarycentricCalculator
{
    private const float Equal = 1f / 3f;
    public TriangleWeight ComputeBarycentricCoordinates(int x, int y, Triangle triangle)
    {
        double area = CalculateTriangleArea(triangle);

        float weight0 = 0f;
        float weight1 = 0f;
        float weight2 = 0f;
        
        if (Math.Abs(area) < RasterizationConstants.BarycentricEpsilon)
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
            
            weight0 = (float)CalculateWeight(point1, point2, x, y, area);
            weight1 = (float)CalculateWeight(point2, point0, x, y, area);
            weight2 = 1 - weight0 - weight1;   
        }
        
        return new TriangleWeight(weight0, weight1, weight2);
    }

    private double CalculateTriangleArea(Triangle triangle)
    {
        Point point0 = triangle.Vertex0.ScreenPoint;
        Point point1 = triangle.Vertex1.ScreenPoint;
        Point point2 = triangle.Vertex2.ScreenPoint;
        
        return (point1.Y - point2.Y) * (point0.X - point2.X) +
               (point2.X - point1.X) * (point0.Y - point2.Y);
    }

    private double CalculateWeight(Point pointA, Point pointB, int x, int y, double area)
    {
        return ((pointA.Y - pointB.Y) * (x - pointB.X) +
                (pointB.X - pointA.X) * (y - pointB.Y)) / area;
    }

    public bool IsPointInTriangle(int x, int y, Triangle triangle)
    {
        Point point0 = triangle.Vertex0.ScreenPoint;
        Point point1 = triangle.Vertex1.ScreenPoint;
        Point point2 = triangle.Vertex2.ScreenPoint;
        
        double sign1 = CalculateCrossProduct(point0, point1, x, y);
        double sign2 = CalculateCrossProduct(point1, point2, x, y);
        double sign3 = CalculateCrossProduct(point2, point0, x, y);
        
        return (sign1 >= 0 && sign2 >= 0 && sign3 >= 0) ||
               (sign1 <= 0 && sign2 <= 0 && sign3 <= 0);
    }

    private double CalculateCrossProduct(Point pointA, Point pointB, int x, int y)
    {
        return (pointB.X - pointA.X) * (y - pointA.Y) -
               (pointB.Y - pointA.Y) * (x - pointA.X);
    }
}