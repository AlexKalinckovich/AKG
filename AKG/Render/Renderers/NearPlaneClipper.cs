using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Model.Vertex;

namespace AKG.Render.Renderers;

public static class NearPlaneClipper
{
    private const float NearPlaneW = 0.5f;
    
    public static List<Triangle> ClipTriangle(Triangle triangle)
    {
        // Проверяем ВСЕ вершины на W < NearPlaneW
        var vertices = new[] { triangle.Vertex0, triangle.Vertex1, triangle.Vertex2 };
        
        // Считаем, сколько вершин нужно клипнуть
        bool[] needsClip = vertices.Select(v => v.ClipPosition.W < NearPlaneW).ToArray();
        int clipCount = needsClip.Count(n => n);
        
        // Все вершины OK
        if (clipCount == 0)
            return new List<Triangle> { triangle };
        
        // Все вершины за near plane
        if (clipCount == 3)
            return new List<Triangle>();
        
        // Одна или две вершины за near plane — клипаем
        return ClipWithNearPlane(triangle, needsClip);
    }
    
    private static List<Triangle> ClipWithNearPlane(Triangle triangle, bool[] needsClip)
    {
        // Разделяем вершины на "хорошие" и "плохие"
        var inside = new List<VertexData>();
        var outside = new List<VertexData>();
        
        var vertices = new[] { triangle.Vertex0, triangle.Vertex1, triangle.Vertex2 };
        for (int i = 0; i < 3; i++)
        {
            if (needsClip[i])
                outside.Add(vertices[i]);
            else
                inside.Add(vertices[i]);
        }
        
        if (inside.Count == 1)
        {
            // Одна вершина внутри, две снаружи
            var newV1 = InterpolateToNearPlaneCorrect(inside[0], outside[0]);
            var newV2 = InterpolateToNearPlaneCorrect(inside[0], outside[1]);
            return new List<Triangle> { new Triangle(inside[0], newV1, newV2) };
        }
        else // inside.Count == 2
        {
            // Две вершины внутри, одна снаружи
            var newV0 = InterpolateToNearPlaneCorrect(inside[0], outside[0]);
            var newV1 = InterpolateToNearPlaneCorrect(inside[1], outside[0]);
            
            return new List<Triangle>
            {
                new Triangle(inside[0], inside[1], newV0),
                new Triangle(inside[1], newV0, newV1)
            };
        }
    }
    
    private static VertexData InterpolateToNearPlaneCorrect(VertexData inside, VertexData outside)
    {
        float t = (NearPlaneW - inside.ClipPosition.W) / 
                  (outside.ClipPosition.W - inside.ClipPosition.W);
        
        // Важно: не клампить, t может быть > 1 если inside ближе к камере чем near plane
        // Но мы уже проверили, что inside.W >= NearPlaneW и outside.W < NearPlaneW
        
        // Правильная перспективная интерполяция
        float invW_inside = 1.0f / inside.ClipPosition.W;
        float invW_outside = 1.0f / outside.ClipPosition.W;
        float invW_new = invW_inside + t * (invW_outside - invW_inside);
        float W_new = 1.0f / invW_new;
        
        float weight_inside = (1.0f - t) * invW_inside / invW_new;
        float weight_outside = t * invW_outside / invW_new;
        
        // Screen position интерполируем линейно (она уже в screen space)
        double screenX = inside.ScreenPoint.X + t * (outside.ScreenPoint.X - inside.ScreenPoint.X);
        double screenY = inside.ScreenPoint.Y + t * (outside.ScreenPoint.Y - inside.ScreenPoint.Y);
        
        return new VertexData(
            inside.WorldPosition * weight_inside + outside.WorldPosition * weight_outside,
            inside.ViewPosition * weight_inside + outside.ViewPosition * weight_outside,
            new Vector4(
                inside.ClipPosition.X + t * (outside.ClipPosition.X - inside.ClipPosition.X),
                inside.ClipPosition.Y + t * (outside.ClipPosition.Y - inside.ClipPosition.Y),
                inside.ClipPosition.Z + t * (outside.ClipPosition.Z - inside.ClipPosition.Z),
                W_new),
            new Point(screenX, screenY),
            inside.Depth * weight_inside + outside.Depth * weight_outside,
            Vector3.Normalize(inside.Normal * weight_inside + outside.Normal * weight_outside),
            inside.UV * weight_inside + outside.UV * weight_outside
        );
    }
}