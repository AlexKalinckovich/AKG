using System.Numerics;
using System.Windows;
using AKG.Model;
using AKG.Model.Vertex;

namespace AKG.Matrix;

public sealed class VertexTransformCalculator
{
    private readonly TransformationMatrixManager _matrixManager;
    private readonly ScreenSpaceConverter _screenSpaceConverter;

    public VertexTransformCalculator(TransformationMatrixManager matrixManager, 
                                     int viewportWidth, int viewportHeight)
    {
        _matrixManager = matrixManager;
        _screenSpaceConverter = new ScreenSpaceConverter(viewportWidth, viewportHeight);

    }

    public VertexData Transform(Vector4 vertex,Vector3 normal, Vector2 uv)
    {
        Vector3 worldNormal = CalculateWorldNormal(normal);
        
        Vector3 vec3Vertex = new Vector3(vertex.X, vertex.Y, vertex.Z);
        
        Vector3 worldPosition = Vector3.Transform(vec3Vertex, _matrixManager.ModelMatrix);
        Vector3 viewPosition = Vector3.Transform(worldPosition, _matrixManager.ViewMatrix);
        Vector4 clipPosition = Vector4.Transform(viewPosition, _matrixManager.ProjectionMatrix);
        
        
        Point screenPoint = _screenSpaceConverter.ConvertToScreenSpace(clipPosition);
        
        return new VertexData(
            worldPosition,
            viewPosition,
            clipPosition,
            screenPoint,
            clipPosition.W != 0 ? clipPosition.Z / clipPosition.W : 0,
            worldNormal,
            uv
        );
    }
    
    private Vector3 CalculateWorldNormal(Vector3 normal)
    {
        Vector3 worldNormal = Vector3.TransformNormal(normal, _matrixManager.ModelMatrix);
        return Vector3.Normalize(worldNormal);
    }
}