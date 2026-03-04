using System.Numerics;
using System.Windows;
using AKG.Model;

namespace AKG.Matrix;

public sealed class VertexTransformCalculator
{
    private readonly TransformationMatrixManager _matrixManager;
    private readonly ScreenSpaceConverter _screenSpaceConverter;
    public VertexTransformCalculator(TransformationMatrixManager matrixManager, 
                                     ScreenSpaceConverter screenSpaceConverter)
    {
        _matrixManager = matrixManager;
        _screenSpaceConverter = screenSpaceConverter;
    }

    public VertexData Transform(Vector4 vertex,Vector3 normal)
    {
        Vector3 worldNormal = CalculateWorldNormal(normal);
        
        Vector4 worldPosition = Vector4.Transform(vertex, _matrixManager.ModelMatrix);
        Vector4 viewPosition = Vector4.Transform(worldPosition, _matrixManager.ViewMatrix);
        Vector4 clipPosition = Vector4.Transform(viewPosition, _matrixManager.ProjectionMatrix);
        
        Point screenPoint = _screenSpaceConverter.ConvertToScreenSpace(clipPosition);
        
        return new VertexData
        {
            WorldPosition = new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z),
            ViewPosition = new Vector3(viewPosition.X, viewPosition.Y, viewPosition.Z),
            ClipPosition = clipPosition,
            ScreenPoint = screenPoint,
            Depth = clipPosition.W != 0 ? clipPosition.Z / clipPosition.W : 0,
            Normal = worldNormal
        };
    }
    
    private Vector3 GetNormal(IReadOnlyList<Vector3> normals, int index)
    {
        return normals.Count > 0 ? normals[index] : Vector3.UnitZ;
    }

    private Vector3 CalculateWorldNormal(Vector3 normal)
    {
        Vector3 worldNormal = Vector3.TransformNormal(normal, _matrixManager.ModelMatrix);
        return Vector3.Normalize(worldNormal);
    }
}